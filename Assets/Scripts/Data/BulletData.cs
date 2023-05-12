using Unity.Entities;

namespace KC.Data {
	public struct BulletData : IComponentData {
		public Entity bulletPrefab;
		public float force;
		public float secondsBetweenShots;
	}
}
