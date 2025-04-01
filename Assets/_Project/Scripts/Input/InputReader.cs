using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Timelesss
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction Roll = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction InterAction = delegate { };
        public event UnityAction Consume = delegate { };

        PlayerInputActions inputActions;

        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions != null) return;

            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }
        public void DisablePlayerActions()
        {
            inputActions.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnFire(InputAction.CallbackContext context)
        {
            Attack?.Invoke();
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    DisableMouseControlCamera?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    EnableMouseControlCamera?.Invoke();
                    break;
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Dash?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash?.Invoke(false);
                    break;
            }
        }

        public void OnRoll(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                Roll?.Invoke();
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                InterAction?.Invoke();
            }
        }
        void IPlayerActions.OnConsume(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Consume?.Invoke();
            }
        }
    }
}