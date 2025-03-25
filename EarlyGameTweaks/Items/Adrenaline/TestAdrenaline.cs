using System.Collections.Generic;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables;
using MapEditorReborn.Commands.ModifyingCommands.Position;
using MEC;
using ServerTools;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class TestAdrenaline : CustomItem
    {
        public override uint Id { get; set; } = 7777;
        public override string Name { get; set; } = "Test";
        public override string Description { get; set; } = "test";
        public override float Weight { get; set; } = 0.5f;
        public PathFinder pf = new PathFinder(stepSize: 1.0f);
        public EventHandlers _eh;

        public override SpawnProperties SpawnProperties { get; set; }

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingInjection;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingInjection;

            base.UnsubscribeEvents();
        }

        private void OnUsingInjection(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Vector3 startPos = ev.Player.Position;
            Vector3 targetPos = Room.Get(RoomType.Lcz914).Position;

            pf.StartPathfinding(startPos, targetPos, (List<Vector3> path) =>
            {
                if (path != null)
                {
                    Log.Info("Path found!");
                    foreach (Vector3 point in path)
                    {
                        Primitive primitive = Primitive.Create(PrimitiveType.Cube);
                        primitive.Flags = PrimitiveFlags.Visible;
                        primitive.Position = point;
                        primitive.Scale = new Vector3(0.3f, 0.3f, 0.3f);
                        primitive.Color = new Color(10f, 0f, 0f, 0.1f);
                    }
                }
                else
                {
                    Log.Info("No path found.");
                }
            });

            //foreach (Vector3 p in path)
            //{
            //   Primitive primitive = Primitive.Create(PrimitiveType.Cube);
            //    primitive.Flags = PrimitiveFlags.Visible;
            //    primitive.Position = p + Vector3.down;
            //    primitive.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            //    primitive.Color = new Color(10f, 0f, 0f, 0.1f);
            //}



            //NavMeshPath path = new NavMeshPath();

            //if (NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, path))
            //{
            //    Log.Info("Path calculated!");
            //    PlacePrimitivesAlongPath(path);
            //}
            //else
            //{
            //    Log.Info("No valid path found.");
            //}
        }

        private void PlacePrimitivesAlongPath(NavMeshPath path)
        {
            foreach (Vector3 point in path.corners)
            {
                Log.Debug(point);
                Primitive primitive = Primitive.Create(PrimitiveType.Cube);
                primitive.Flags = PrimitiveFlags.Visible;
                primitive.Position = point;
                primitive.Scale = new Vector3(1f, 1f, 1f);
                primitive.Color = new Color(10f, 0f, 0f, 0.1f);
            }
        }
    }
}
