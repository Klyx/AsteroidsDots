using KC.Tags;
using Unity.Entities;
using UnityEngine;

namespace KC.Authoring {
    public sealed class BulletAuthoring : MonoBehaviour {
        private class Baker : Baker<BulletAuthoring> {
            public override void Bake(BulletAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent<BulletTag>(entity);
            }
        }
    }
}