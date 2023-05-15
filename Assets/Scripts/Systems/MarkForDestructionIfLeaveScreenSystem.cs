using KC.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace KC.Systems {
    [BurstCompile, UpdateInGroup(typeof(LateSimulationSystemGroup))]
	public sealed partial class MarkForDestructionIfLeaveScreenSystem : SystemBase {
		private Camera _camera;
		
		protected override void OnUpdate() {
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
			
			foreach (var (transform, entity) in
			         SystemAPI.Query<RefRW<LocalTransform>>().WithNone<ScreenWarpTag>().WithEntityAccess()) {
				float3 viewportPosition = _camera.WorldToViewportPoint(transform.ValueRO.Position);
				if (!IsOutsideOfScreen(viewportPosition)) {
					continue;
				}
				
				ecb.AddComponent<DestroyEntityTag>(entity);
			}
			
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}
		
		protected override void OnStartRunning() {
			_camera = Camera.main;
		}

		[BurstCompile]
		protected override void OnStopRunning() {
		}

		[BurstCompile]
		private bool IsOutsideOfScreen(float3 viewportPosition) {
			return (viewportPosition.x > 1f || viewportPosition.x < 0f || viewportPosition.y > 1 ||
			        viewportPosition.y < 0f);
		}
	}
}
