using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Game.Character
{
    [RequireComponent(typeof(CharacterMovement))]
    public class CharacterInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        [Header("Options")]
        [SerializeField] private bool useGamepadIfAvailable = true;
        [SerializeField] private CursorManager cursorManager;

        private CharacterMovement movement;
        private AnimationController animations;
        private AbilityScript ability;
        private InputSystem_Actions actions;

        private void Awake()
        {
            actions = new InputSystem_Actions();
            actions.Player.SetCallbacks(this);
        }

        private void Start()
        {
            movement = GetComponent<CharacterMovement>();
            animations = GetComponent<AnimationController>();
            ability = GetComponent<AbilityScript>();
            
        }

        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            movement.SetMoveInput(input);

            movement.SetIsWalking(context.performed);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // Pas utilisé pour le moment (caméra ou orientation plus tard)
        }

        public void OnEchap(InputAction.CallbackContext context)
        {
            // Pas utilisé pour le moment (caméra ou orientation plus tard)
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            animations.Attack();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            // À implémenter si besoin
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            // À implémenter si besoin
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // À implémenter si besoin
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            // Changement d'arme/slot etc. plus tard
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            // Changement d'arme/slot etc. plus tard
        }

        public void OnAbility(InputAction.CallbackContext context)
        {
            ability.SlowTime();

            animations.TimeAbility();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
                movement.SetIsSprinting(true);

            if (context.canceled)
                movement.SetIsSprinting(false);
        }
    }
}

