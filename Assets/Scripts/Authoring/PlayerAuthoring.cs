using KC.GameData;
using KC.Tags;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

namespace KC.Authoring {
	public sealed class PlayerAuthoring : MonoBehaviour {
		[SerializeField] private MovementGameData _movementGameData;

		private class Baker : Baker<PlayerAuthoring> {
			public override void Bake(PlayerAuthoring authoring) {
				Assert.IsNotNull(authoring._movementGameData, "authoring._movementGameData != null");
				
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent<PlayerTag>(entity);
				
				AddComponent(entity, new MovementData() {
					linearImpulseForce = authoring._movementGameData.linearImpulseForce,
					angularSpeed = authoring._movementGameData.angularSpeed
				});
			}
		}
	}
}
