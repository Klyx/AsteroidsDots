using KC.Tags;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;

namespace KC.Systems {
    [BurstCompile, UpdateInGroup(typeof(LateSimulationSystemGroup)), 
     UpdateAfter(typeof(MarkForDestructionIfLeaveScreenSystem))]
	public partial struct DestroyEntitySystem : ISystem {
		[BurstCompile]
		public void OnCreate(ref SystemState state) {
			state.RequireForUpdate<DestroyEntityTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state) {
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
			
			foreach ((DestroyEntityTag _, Entity entity) in 
			         SystemAPI.Query<DestroyEntityTag>().WithEntityAccess()) {
				ecb.DestroyEntity(entity);
			}
			
			ecb.Playback(state.EntityManager);
			ecb.Dispose();
		}
		
		[BurstCompile]
		public void OnDestroy(ref SystemState state) {
		}
	}
}
