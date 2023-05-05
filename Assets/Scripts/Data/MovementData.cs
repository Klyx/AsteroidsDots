using Unity.Entities;

namespace KC {
	public struct MovementData : IComponentData {
		public float maxLinearVelocity;
		public float linearImpulseForce;
		public float angularSpeed;
	}
}
