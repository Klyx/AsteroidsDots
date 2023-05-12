using KC.Data;
using KC.GameData;
using KC.Tags;
using Unity.Entities;
using UnityEngine;
using Unity.Assertions;

namespace KC.Authoring
{
    public sealed class AsteroidAuthoring : MonoBehaviour {
        private enum AsteroidSize {
            Big,
            Medium,
            Small
        }
        
        [SerializeField] private MovementGameData _movementGameData;
        [SerializeField] private AsteroidSize _asteroidSize;

        private sealed class Baker : Baker<AsteroidAuthoring> {
            public override void Bake(AsteroidAuthoring authoring) {
                Assert.IsNotNull(authoring._movementGameData, "authoring._movementGameData != null");
                
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<ScreenWarpTag>(entity);
                AddComponent(entity, new MovementData() {
                    linearImpulseForce = authoring._movementGameData.linearImpulseForce,
                    angularSpeed = authoring._movementGameData.angularSpeed
                });

                SetAsteroidSizeTag(authoring, ref entity);
            }

            private void SetAsteroidSizeTag(AsteroidAuthoring authoring, ref Entity entity) {
                switch (authoring._asteroidSize) {
                    case AsteroidSize.Big:
                        AddComponent<AsteroidBig>(entity);
                        break;
                    case AsteroidSize.Medium:
                        AddComponent<AsteroidMedium>(entity);
                        break;
                    case AsteroidSize.Small:
                        AddComponent<AsteroidSmall>(entity);
                        break;
                }
            }
        }
    }
}