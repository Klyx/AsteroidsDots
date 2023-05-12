using Unity.Entities;

namespace KC {
	public struct SpawnerData : IComponentData {
		public Entity bigAsteroidPrefab;
		public Entity mediumAsteroidPrefab;
		public Entity smallAsteroidPrefab;

		public float spawnTimeInSeconds;
		public float spawnAmount;
		public float spawnSafeAreaRadius;

		public float nextSpawnTime;
	}
}
