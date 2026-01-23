using UnityEngine;

namespace Game.Character
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Vitesse")]
        [SerializeField] private float baseMoveSpeed = 3f;
        [SerializeField] private float sprintMultiplier = 1.5f;

        [SerializeField] private Camera cam;

        private Rigidbody _rb;
        private AnimationController _animator;

        private Vector2 _moveInput;

        private bool _movementEnabled = true;

        private float _externalSpeedModifier = 1f;

        private bool _isSprinting;
        private bool _isWalking;

        private float gravity = 9.98f;


        public Vector2 MoveInput => _moveInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            _animator = GetComponent<AnimationController>();

        }

        private void Start()
        {
            _rb.useGravity = false;
            _rb.linearVelocity = transform.forward * gravity / Time.timeScale;
        }

        private void FixedUpdate()
        {
            HandleMovement();
            fixDirection();

            _rb.linearVelocity += Physics.gravity / Time.timeScale * Time.fixedDeltaTime;
        }

        public void SetMoveInput(Vector2 input)
        {
            _moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        public void SetIsSprinting(bool isSprinting)
        {
            _animator.Sprint(isSprinting);
        }
        
        public bool IsSprinting()
        {
            return _isSprinting;
        }

        public void SetIsWalking(bool isWalking)
        {
            _animator.Walk(isWalking);
        }

        public void fixDirection()
        {
            Vector3 camEuler = cam.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);

        }

        public void EnableMovement(bool enabled)
        {
            _movementEnabled = enabled;

            if (!enabled)
            {
                _moveInput = Vector2.zero;
                Vector3 v = _rb.linearVelocity;
                v.x = 0f;
                v.z = 0f;
                _rb.linearVelocity = v;
            }
        }

        public void SetSpeedModifier(float modifier)
        {
            _externalSpeedModifier = Mathf.Max(0f, modifier);
        }

        private void HandleMovement()
        {
            if (!_movementEnabled)
                return;

            Vector2 input = _moveInput;

            float speedFactor = 1f;

            if (_isSprinting)
            {
                speedFactor *= sprintMultiplier;
            }

            float finalSpeed = baseMoveSpeed * speedFactor * _externalSpeedModifier;

            if (Time.timeScale > 0f) finalSpeed /= Time.timeScale;

            Vector3 moveDir = transform.forward * input.y + transform.right * input.x;

            Vector3 targetVelocity = moveDir.normalized * finalSpeed;

            targetVelocity.y = _rb.linearVelocity.y;

            _rb.linearVelocity = targetVelocity;
        }
    }
}
