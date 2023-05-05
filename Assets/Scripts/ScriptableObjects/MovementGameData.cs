using UnityEngine;

namespace KC.GameData {
	[CreateAssetMenu(fileName = "MovementGameData", menuName = "KC/Movement data")]
	public sealed class MovementGameData : ScriptableObject {
		[Header("Moving")]
		public float maxLinearVelocity = 1f;
		public float linearImpulseForce = 1f;

		[Header("Turning")]
		public float angularSpeed = 1f;
	}
}
