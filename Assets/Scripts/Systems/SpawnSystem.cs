using KC.Data;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Physics;

namespace KC.Systems {
    [BurstCompile]
	public partial struct SpawnSystem : ISystem, ISystemStartStop {
		private Random _random;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state) {
		}
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state) {
			RefRW<SpawnerData> spawnerData = SystemAPI.GetSingletonRW<SpawnerData>();

			if (spawnerData.ValueRO.nextSpawnTime >= SystemAPI.Time.ElapsedTime) {
				return;
			}

			for (int i = 0; i < spawnerData.ValueRO.spawnAmount; i++)
			{
				GetPrefabToInstantiate(spawnerData, out Entity entityPrefab);
				Entity entity = state.EntityManager.Instantiate(entityPrefab);
				SetupAsteroid(ref state, ref entity);
				
				spawnerData.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + 
				                                    spawnerData.ValueRO.spawnTimeInSeconds;
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state) {
		}

		[BurstCompile]
		public void OnStartRunning(ref SystemState state) {
			RefRW<SpawnerData> spawnerData = SystemAPI.GetSingletonRW<SpawnerData>();
			spawnerData.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + 
			                                    spawnerData.ValueRO.spawnTimeInSeconds;
			
			_random.InitState();
		}

		[BurstCompile]
		public void OnStopRunning(ref SystemState state) {
		}

		[BurstCompile]
		private void SetupAsteroid(ref SystemState state, ref Entity entity) {
			RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity, false);
			RefRO<MovementData> movementData = SystemAPI.GetComponentRW<MovementData>(entity, true);

			float3 direction = GetRandomVelocity();
			velocity.ValueRW.Linear = direction * movementData.ValueRO.linearImpulseForce;
		}

		[BurstCompile]
		private float3 GetRandomVelocity() {
			float angle = _random.NextFloat(0f, math.PI * 2f);
			return math.normalize(new float3(math.cos(angle), 0f, math.sin(angle)));
		}

		[BurstCompile]
		private void GetPrefabToInstantiate(RefRW<SpawnerData> spawnerData, out Entity entity) {
			switch (_random.NextInt(0, 3)) {
				case 0:
					entity = spawnerData.ValueRO.bigAsteroidPrefab;
					break;
				case 1:
					entity = spawnerData.ValueRO.mediumAsteroidPrefab;
					break;
				default:
					entity = spawnerData.ValueRO.smallAsteroidPrefab;
					break;
			}
		}
	}
}
