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

        private readonly InputControls _inputControls = new();
        private float _moveInput;
        private float _turnInput;

        [BurstCompile]
        protected override void OnCreate() {
        }

        [BurstCompile]
        protected override void OnUpdate() {
            RefRO<LocalToWorld> localToWorld = SystemAPI.GetComponentRW<LocalToWorld>(_playerEntity, true);
            RefRW<PhysicsVelocity> velocity = SystemAPI.GetComponentRW<PhysicsVelocity>(_playerEntity, false);
            RefRO<PhysicsMass> mass = SystemAPI.GetComponentRW<PhysicsMass>(_playerEntity, true);
            RefRW<MovementData> movementData = SystemAPI.GetComponentRW<MovementData>(_playerEntity, false);

            float deltaTime = SystemAPI.Time.fixedDeltaTime;

            velocity.ValueRW.Angular = math.up() * _turnInput * (movementData.ValueRO.angularSpeed * deltaTime);

            if (_moveInput == 0f) {
                return;
            }
            
            velocity.ValueRW.ApplyLinearImpulse(in mass.ValueRO,
                math.forward(localToWorld.ValueRO.Rotation) * movementData.ValueRO.linearImpulseForce *
                deltaTime);
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
    }
}