using KC.Data;
using KC.Input;
using KC.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KC.Systems {
    [BurstCompile, UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))]
    public sealed partial class PlayerSystem : SystemBase, InputControls.IPlayActions {
        private Entity _playerEntity;

        private float _linearSpeed;
        private double _nextShotTime;

        private readonly InputControls _inputControls = new();
        private float _moveInput;
        private float _turnInput;
        private bool _shooInput;
        
        private const float BulletOffset = 10f;

        [BurstCompile]
        protected override void OnCreate() {
        }

        [BurstCompile]
        protected override void OnUpdate() {
            RefRO<LocalToWorld> localToWorld = SystemAPI.GetComponentRW<LocalToWorld>(_playerEntity, true);
            RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(_playerEntity, false);
            RefRO<PhysicsMass> mass = SystemAPI.GetComponentRW<PhysicsMass>(_playerEntity, true);
            RefRO<MovementData> movementData = SystemAPI.GetComponentRW<MovementData>(_playerEntity, true);

            float deltaTime = SystemAPI.Time.fixedDeltaTime;

            PerformMovement(velocity, movementData, deltaTime, mass, localToWorld);
            PerformShooting(SystemAPI.Time.ElapsedTime);
        }

        protected override void OnStartRunning() {
            _inputControls.Enable();
            _inputControls.Play.SetCallbacks(this);

            _playerEntity = GetEntityQuery(typeof(PlayerTag)).GetSingletonEntity();
        }

        protected override void OnStopRunning() {
            _inputControls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            _moveInput = context.ReadValue<float>();
        }

        public void OnTurn(InputAction.CallbackContext context) {
            _turnInput = context.ReadValue<float>();
        }

        public void OnShoot(InputAction.CallbackContext context) {
            _shooInput = context.performed;
        }
        
        [BurstCompile]
        private void PerformMovement(RefRW<PhysicsVelocity> velocity, RefRO<MovementData> movementData, float deltaTime, 
            RefRO<PhysicsMass> mass, RefRO<LocalToWorld> localToWorld) {
            velocity.ValueRW.Angular = math.up() * _turnInput * 
                                       (movementData.ValueRO.angularSpeed * deltaTime);

            if (_moveInput == 0f) {
                return;
            }

            velocity.ValueRW.ApplyLinearImpulse(in mass.ValueRO,
                math.forward(localToWorld.ValueRO.Rotation) * 
                movementData.ValueRO.linearImpulseForce *
                deltaTime);
        }

        [BurstCompile]
        private void PerformShooting(double elapsedTime) {
            if (!_shooInput || _nextShotTime > elapsedTime) {
                return;
            }

            RefRO<BulletData> bulletData = 
                SystemAPI.GetComponentRW<BulletData>(_playerEntity, true);
            RefRO<LocalTransform> transform = 
                SystemAPI.GetComponentRW<LocalTransform>(_playerEntity, true);
            RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(_playerEntity, true);
            
            Entity bulletEntity = EntityManager.Instantiate(bulletData.ValueRO.bulletPrefab);
            RefRW<LocalTransform> bulletTransform = SystemAPI.GetComponentRW<LocalTransform>(bulletEntity, false);
            RefRW<PhysicsVelocity> bulletVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(bulletEntity, false);

            bulletTransform.ValueRW.Position = transform.ValueRO.Position + transform.ValueRO.Forward() * BulletOffset;
            bulletTransform.ValueRW.Rotation = transform.ValueRO.Rotation;
            // bulletVelocity.ValueRW.Linear = math.max(
            //     transform.ValueRO.Forward() * 
            //     math.length(velocity.ValueRO.Linear) * bulletData.ValueRO.addOnForce,
            //     transform.ValueRO.Forward() * bulletData.ValueRO.addOnForce
            //     );

            bulletVelocity.ValueRW.Linear = transform.ValueRO.Forward() * bulletData.ValueRO.force;

            _nextShotTime = elapsedTime + bulletData.ValueRO.secondsBetweenShots;
        }
    }
}