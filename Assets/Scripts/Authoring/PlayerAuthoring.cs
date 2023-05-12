using KC.Data;
using KC.GameData;
using KC.Tags;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace KC.Authoring {
	public sealed class PlayerAuthoring : MonoBehaviour {
		[Header("Movement")]
		[SerializeField] private MovementGameData _movementGameData;
		
		[Header("Shooting")]
		[SerializeField] private BulletGameData _bulletGameData;
		
		

		private sealed class Baker : Baker<PlayerAuthoring> {
			public override void Bake(PlayerAuthoring authoring) {
				Assert.IsNotNull(authoring._movementGameData, "authoring._movementGameData != null");
				Assert.IsNotNull(authoring._bulletGameData, "authoring._bulletGameData != null");
				
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent<PlayerTag>(entity);
				AddComponent<ScreenWarpTag>(entity);
				
				AddComponent(entity, new MovementData() {
					linearImpulseForce = authoring._movementGameData.linearImpulseForce,
					angularSpeed = authoring._movementGameData.angularSpeed
				});
				
				AddComponent(entity, new BulletData()
				{
					bulletPrefab = GetEntity(authoring._bulletGameData.bulletPrefab, TransformUsageFlags.Dynamic),
					force = authoring._bulletGameData.force,
					secondsBetweenShots = 60f / authoring._bulletGameData.roundsPerMinute
				});
			}
		}
	}
}
