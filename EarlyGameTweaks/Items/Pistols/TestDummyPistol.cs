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
        public override string Name { get; set; } = "[REDACTED]";
        public override string Description { get; set; } = "[REDACTED]";
        public override float Damage { get; set; } = 30;
        public override byte ClipSize { get; set; } = 50;
        public override float Weight { get; set; } = 0.1f;
        public Transform handBone;
        private Queue<Primitive> spawnedPrimitives = new Queue<Primitive>();
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside049Armory,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying += VaporizeOnKill;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Dying -= VaporizeOnKill;
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

                SpawnBeam(handBone.position, raycastHit.point);
                SpawnSpiralAroundBeam(handBone.position, raycastHit.point);
            }
        }

        public void VaporizeOnKill(DyingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Attacker == null) return;

            if (Check(ev.Attacker.CurrentItem))
            {
                if (ev.DamageHandler.Type == DamageType.Crossvec) {
                    ev.Player.Vaporize();
                }
            }
        }

        private IEnumerator<float> FadeOutAndDestroy(Primitive primitive, float duration)
        {
            float elapsed = 0f;
            Color initialColor = primitive.Color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Farbe bleibt gleich, nur Transparenz nimmt ab
                float alpha = Mathf.Lerp(initialColor.a, 0f, elapsed / duration);
                primitive.Color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

                yield return Timing.WaitForOneFrame;
            }

            primitive.Destroy();
        }

        public void SpawnBeam(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 midPoint = start + direction * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(direction);

            Primitive beam = Primitive.Create(PrimitiveType.Cylinder);
            beam.Flags = PrimitiveFlags.Visible;

            // Farbe: Blitz-Blau mit Transparenz
            beam.Color = new Color(10f, 0f, 0f, 0.9f);

            // Setze Position, Rotation und Skalierung des Zylinders
            beam.Position = midPoint;
            beam.Rotation = rotation * Quaternion.Euler(90f, 0f, 0f); // Zylinder liegt normalerweise entlang Y-Achse, wir rotieren ihn auf Z
            beam.Scale = new Vector3(0.03f, distance / 2f, 0.03f); // Scale.y = halbe Länge

            spawnedPrimitives.Enqueue(beam);
            Timing.RunCoroutine(FadeOutAndDestroy(beam, 2f));
        }

        private void SpawnSpiralAroundBeam(Vector3 start, Vector3 end, float duration = 2f)
        {
            Vector3 beamDirection = end - start;
            float beamLength = beamDirection.magnitude;
            Vector3 beamForward = beamDirection.normalized;

            int segments = 100; // mehr = glatter
            float radius = 0.1f;
            float turns = 4f;

            Quaternion beamRotation = Quaternion.FromToRotation(Vector3.up, beamForward);

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;
                float angle = t * turns * 2f * Mathf.PI;
                float y = t * beamLength;

                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                Vector3 localPos = new Vector3(x, y, z);
                Vector3 worldPos = start + beamRotation * localPos;

                float nextT = (i + 1) / (float)segments;
                float nextAngle = nextT * turns * 2f * Mathf.PI;
                float nextY = nextT * beamLength;
                Vector3 nextLocalPos = new Vector3(Mathf.Cos(nextAngle) * radius, nextY, Mathf.Sin(nextAngle) * radius);
                Vector3 nextWorldPos = start + beamRotation * nextLocalPos;

                Vector3 tangent = (nextWorldPos - worldPos).normalized;
                float segmentLength = Vector3.Distance(nextWorldPos, worldPos);

                Primitive spiralSegment = Primitive.Create(PrimitiveType.Cylinder);
                spiralSegment.Flags = PrimitiveFlags.Visible;
                spiralSegment.Color = new Color(0f, 0f, 25f, 0.9f);

                // Scale.y = halbe Länge
                spiralSegment.Scale = new Vector3(0.06f, segmentLength / 2f, 0.06f);
                spiralSegment.Position = (worldPos + nextWorldPos) / 2f;
                spiralSegment.Rotation = Quaternion.LookRotation(tangent) * Quaternion.Euler(90f, 0f, 0f);

                spawnedPrimitives.Enqueue(spiralSegment);
                Timing.RunCoroutine(FadeOutAndDestroy(spiralSegment, duration));
            }
        }
    }
}
