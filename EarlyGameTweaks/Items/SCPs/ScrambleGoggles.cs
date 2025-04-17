using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP268)]
    public class ScrambleGoggles : GogglesItem
    {
        public override uint Id { get; set; } = 804;
        public override string Name { get; set; } = "Scramble Goggles";
        public override string Description { get; set; } = "yup";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; }
        private readonly Dictionary<Player, List<Primitive>> scrambledCubes = new();
        private readonly Dictionary<Player, List<Vector3>> cubeInitialPositions = new(); // Speichern der Ursprungspositionen

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void EquipGoggles(Player player, bool showMessage = true)
        {
            base.EquipGoggles(player, showMessage);

            if (!scrambledCubes.ContainsKey(player))
                scrambledCubes[player] = new List<Primitive>();

            if (!cubeInitialPositions.ContainsKey(player))
                cubeInitialPositions[player] = new List<Vector3>();

            // Erstelle Primitives für alle SCP-096-Spieler
            foreach (Player scp in Player.List)
            {
                if (scp.Role.Type != RoleTypeId.Scp096)
                    continue;

                // Animator und Transform holen
                Animator animator = scp.GameObject.GetComponentInChildren<Animator>(true);
                Transform[] allTransforms = scp.GameObject.GetComponentsInChildren<Transform>(true);
                Transform headTransform = allTransforms.FirstOrDefault(t => t.name == "head");

                if (headTransform == null) continue; // Falls der Kopf nicht gefunden wird, überspringen

                // Startposition und Richtung definieren
                Vector3 start = headTransform.position;
                Vector3 direction = scp.CameraTransform.forward.normalized;

                // Erzeuge Primitives und speichere ihre Ursprungspositionen
                for (int i = 0; i < 10; i++)
                {
                    Vector3 spawnPosition = start + direction * i * 0.05f;

                    Primitive primitive = Primitive.Create(PrimitiveType.Cube);
                    primitive.Flags = PrimitiveFlags.Visible;
                    primitive.Color = new Color(24.1f / 255f, 25.7f / 255f, 50.0f / 255f, 0.1f);
                    primitive.Position = spawnPosition;
                    primitive.Scale *= 0.05f;

                    scrambledCubes[player].Add(primitive);
                    cubeInitialPositions[player].Add(spawnPosition); // Speichern der Ursprungsposition

                    // Entferne Primitives für andere Spieler
                    foreach (Player p in Player.List)
                    {
                        if (p != player)
                            p.ODestroyNetworkIdentity(primitive.AdminToyBase.netIdentity);
                    }
                }

                // Starte die kontinuierliche Animation der Primitives
                StartCoroutineForPlayer(scp);
            }
        }

        protected override void RemoveGoggles(Player player, bool showMessage = true)
        {
            base.RemoveGoggles(player, showMessage);

            // Zerstöre alle Primitives, wenn die Brille entfernt wird
            if (scrambledCubes.TryGetValue(player, out var cubes))
            {
                foreach (Primitive cube in cubes)
                {
                    cube.Destroy();
                }

                scrambledCubes.Remove(player);
            }

            // Entferne alle Ursprungspositionen
            if (cubeInitialPositions.ContainsKey(player))
            {
                cubeInitialPositions[player].Clear();
            }
        }

        private void StartCoroutineForPlayer(Player scp)
        {
            // Starte die Coroutine für diesen SCP-Spieler
            if (scrambledCubes.ContainsKey(scp) && scrambledCubes[scp].Count > 0)
            {
                Timing.RunCoroutine(UpdatePrimitiveMovement(scp), Segment.FixedUpdate);
            }
        }

        private IEnumerator<float> UpdatePrimitiveMovement(Player scp)
        {
            // Hole die Liste der Primitives und ihre Ursprungspositionen
            if (!scrambledCubes.ContainsKey(scp)) yield break;

            List<Primitive> primitives = scrambledCubes[scp];
            List<Vector3> initialPositions = cubeInitialPositions[scp];

            // Bewege die Primitives kontinuierlich
            while (true)
            {
                for (int i = 0; i < primitives.Count; i++)
                {
                    // Berechne eine einfache Bewegung entlang der Blickrichtung
                    float moveSpeed = 0.1f; // Geschwindigkeit der Bewegung
                    Vector3 moveDirection = scp.CameraTransform.forward.normalized * moveSpeed;

                    // Aktualisiere die Position jedes Primitives entlang der Blickrichtung
                    Vector3 newPosition = initialPositions[i] + moveDirection * (i + 1);
                    primitives[i].Position = newPosition; // Position des Primitives anpassen
                }

                // Warte, bevor du die nächste Aktualisierung machst
                yield return Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
