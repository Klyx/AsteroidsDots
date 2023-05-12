using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace KC.Systems
{
    [BurstCompile, UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerSystem))]
    public sealed partial class ScreenWarpSystem : SystemBase {
        private Camera _camera;
        private EntityQuery _entityQuery;
        
        private const float TeleportOffset = 0.05f;

        protected override void OnUpdate() {
            foreach (var transform in
                     SystemAPI.Query<RefRW<LocalTransform>>().WithAll<ScreenWarpTag>())
            {
                float3 worldPosition = transform.ValueRO.Position;
                float3 viewportPosition = _camera.WorldToViewportPoint(worldPosition);

                WarpX(viewportPosition.x, ref worldPosition);
                WarpY(viewportPosition.y, ref worldPosition);
                
                transform.ValueRW.Position = worldPosition;
            }
        }

        protected override void OnStartRunning() {
            _camera = Camera.main;
        }

        [BurstCompile]
        protected override void OnStopRunning() {
        }

        [BurstCompile]
        private void WarpX(float viewportPositionX, ref float3 worldPosition) {
            if (viewportPositionX + math.EPSILON > 1f) {
                worldPosition.x = -worldPosition.x + TeleportOffset;
            }
            else if (viewportPositionX - math.EPSILON < 0f) {
                worldPosition.x = -worldPosition.x - TeleportOffset;
            }
        }

        [BurstCompile]
        private void WarpY(float viewportPositionY, ref float3 worldPosition) {
            if (viewportPositionY > 1f) {
                worldPosition.z = -worldPosition.z + TeleportOffset;
            }
            else if (viewportPositionY < 0f) {
                worldPosition.z = -worldPosition.z - TeleportOffset;
            }
        }
    }
}