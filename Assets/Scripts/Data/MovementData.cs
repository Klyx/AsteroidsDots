using Unity.Entities;

namespace KC.Data {
	public struct MovementData : IComponentData {
		public float linearImpulseForce;
		public float angularSpeed;
	}
}
