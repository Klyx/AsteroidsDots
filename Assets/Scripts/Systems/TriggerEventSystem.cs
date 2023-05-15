using KC.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace KC.Systems {
    [BurstCompile, UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), 
     UpdateAfter(typeof(PhysicsSystemGroup))]
	public partial struct TriggerEventSystem : ISystem {
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
			state.RequireForUpdate<SimulationSingleton>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state) {
			SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
			EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
				.CreateCommandBuffer(state.WorldUnmanaged);
			
			state.Dependency = new TriggerJob
			{
				ecb = ecb
			}.Schedule(simulation, state.Dependency);
			state.Dependency.Complete();
		}
		
		[BurstCompile]
		public void OnDestroy(ref SystemState state) {
		}

		private struct TriggerJob : ITriggerEventsJob {
			public EntityCommandBuffer ecb;
			
			public void Execute(TriggerEvent triggerEvent) {
				ecb.AddComponent<DestroyEntityTag>(triggerEvent.EntityA);
				ecb.AddComponent<DestroyEntityTag>(triggerEvent.EntityB);
			}
		}
	}
}
