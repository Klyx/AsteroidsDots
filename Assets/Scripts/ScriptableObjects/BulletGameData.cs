using UnityEngine;

namespace KC.GameData {
	[CreateAssetMenu(fileName = "BulletGameData", menuName = "KC/Bullet data")]
	public sealed class BulletGameData : ScriptableObject {
		public GameObject bulletPrefab;
		public float force;
		public float roundsPerMinute;
	}
}
