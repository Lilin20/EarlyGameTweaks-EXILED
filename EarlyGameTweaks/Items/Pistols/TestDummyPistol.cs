using System.Collections.Generic;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunCrossvec)]
    public class TestDummyPistol : CustomWeapon
    {
        public override uint Id { get; set; } = 9000;
        public override string Name { get; set; } = "test";
        public override string Description { get; set; } = "test";
        public override float Damage { get; set; } = 1;
        public override byte ClipSize { get; set; } = 50;
        public override float Weight { get; set; } = 0.1f;
        public Transform handBone;
        private Queue<Primitive> spawnedPrimitives = new Queue<Primitive>();
        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
        protected override void OnShooting(ShootingEventArgs ev)
        {
            
            if (Check(ev.Player.CurrentItem))
            {
                if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                   100, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                    return;

                if (raycastHit.collider is null)
                    return;

                Animator animator = ev.Player.GameObject.GetComponentInChildren<Animator>();
                Transform handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);

                SpawnPrimitivesAlongPath(handBone.position, raycastHit.point);

                SpawnElectricEffect(raycastHit.point);
            }
        }
        private IEnumerator<float> FadeOutAndDestroy(Primitive primitive, float duration)
        {
            float elapsed = 0f;
            Color initialColor = primitive.Color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Leichtes Flackern, damit die Blitze nicht statisch wirken
                float flicker = Mathf.PingPong(elapsed * 15f, 0.3f);

                // Farbverlauf von Blau → Weiß mit Transparenz für einen realistischen Blitz-Effekt
                float alpha = Mathf.Lerp(initialColor.a, 0f, elapsed / duration);
                primitive.Color = new Color(initialColor.r + flicker, initialColor.g + flicker, initialColor.b + flicker, alpha);

                yield return Timing.WaitForOneFrame;
            }

            primitive.Destroy();
        }

        public void SpawnElectricEffect(Vector3 position, int boltCount = 10, float duration = 0.2f)
        {
            for (int i = 0; i < boltCount; i++)
            {
                // Zufällige leichte Versetzung für dynamischere Blitze
                Vector3 randomOffset = Random.insideUnitSphere * 0.15f;

                // ⚡ Erstelle Blitz (Dünne Linie)
                Primitive bolt = Primitive.Create(PrimitiveType.Cylinder);
                bolt.Flags = PrimitiveFlags.Visible;
                bolt.Color = new Color(0.3f, 0.8f, 1f, 1f); // Hellblauer Blitz
                bolt.Position = position + randomOffset;
                bolt.Scale = new Vector3(0.02f, Random.Range(0.3f, 0.6f), 0.02f); // Dünn & lang

                // Zufällige Rotation, damit es wie ein echtes Entladen aussieht
                bolt.Rotation = Quaternion.Euler(Random.Range(-45, 45), Random.Range(0, 360), Random.Range(-45, 45));

                // Animiertes Flackern & Entfernen
                Timing.RunCoroutine(FadeOutAndDestroy(bolt, duration * Random.Range(0.7f, 1.2f)));
            }
        }
        public void SpawnPrimitivesAlongPath(Vector3 start, Vector3 end)
        {
            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            for (float i = 0; i <= distance; i += 0.05f)
            {
                Vector3 spawnPosition = start + direction * i;
                Primitive primitive = Primitive.Create(PrimitiveType.Cube);
                primitive.Flags = PrimitiveFlags.Visible;
                primitive.Color = new Color(24.1f, 25.7f, 50.0f, 0.1f);
                primitive.Position = spawnPosition;
                primitive.Scale *= 0.05f;

                spawnedPrimitives.Enqueue(primitive);

                // Starte Fading & Destroy nach 0.5 Sekunden
                Timing.RunCoroutine(FadeOutAndDestroy(primitive, 2f));
            }
            SpawnSpiralAlongPath(start, end);
        }

        public void SpawnSpiralAlongPath(Vector3 start, Vector3 end, int spiralTurns = 10, float radius = 0.1f)
        {
            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / 0.05f); // Schritte für die Spiralpunkte

            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)steps; // Fortschritt 0-1 entlang der Linie
                Vector3 currentPosition = Vector3.Lerp(start, end, t); // Interpolierte Position

                // 🌀 Spiralbewegung berechnen
                float angle = t * spiralTurns * 360f * Mathf.Deg2Rad; // Spiralwinkel
                float xOffset = Mathf.Cos(angle) * radius;
                float yOffset = Mathf.Sin(angle) * radius;

                // Spirale um die Schusslinie drehen
                Quaternion rotation = Quaternion.LookRotation(direction);
                Vector3 spiralPosition = currentPosition + rotation * new Vector3(xOffset, yOffset, 0);

                // Erstelle die Spirale mit dünnen Linien
                Primitive spiralPrimitive = Primitive.Create(PrimitiveType.Cube);
                spiralPrimitive.Flags = PrimitiveFlags.Visible;
                spiralPrimitive.Color = new Color(50f, 0f, 0f, 0.1f); // Lila-blauer Plasma-Effekt
                spiralPrimitive.Position = spiralPosition;
                spiralPrimitive.Scale = new Vector3(0.025f, 0.025f, 0.025f); // Dünne Linien als Effekt

                // Starte Fading-Effekt
                Timing.RunCoroutine(FadeOutAndDestroy(spiralPrimitive, 2f));
            }
        }
    }
}
