using Unity.Entities;
using UnityEngine;

namespace KC.Authoring {
    public sealed class SpawnAuthoring : MonoBehaviour {
        [Header("Spawn")]
        [SerializeField] private float _spawnTimeInSeconds;
        [SerializeField] private float _spawnAmount;
        [SerializeField] private float _spawnSafeAreaRadius;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _bigAsteroidPrefab;
        [SerializeField] private GameObject _mediumAsteroidPrefab;
        [SerializeField] private GameObject _smallAsteroidPrefab;

        private sealed class Baker : Baker<SpawnAuthoring> {
            public override void Bake(SpawnAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new SpawnerData()
                {
                    spawnTimeInSeconds = authoring._spawnTimeInSeconds,
                    spawnAmount = authoring._spawnAmount,
                    spawnSafeAreaRadius = authoring._spawnSafeAreaRadius,
                    
                    bigAsteroidPrefab = GetEntity(authoring._bigAsteroidPrefab, TransformUsageFlags.Dynamic),
                    mediumAsteroidPrefab = GetEntity(authoring._mediumAsteroidPrefab, TransformUsageFlags.Dynamic),
                    smallAsteroidPrefab = GetEntity(authoring._smallAsteroidPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}