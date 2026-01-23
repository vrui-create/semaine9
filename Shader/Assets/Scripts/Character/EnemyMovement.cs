using UnityEngine;

namespace Game.Character
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Vitesse")]
        [SerializeField] private float baseMoveSpeed = 3f;

        [SerializeField] private float rotationSpeed = 10f;


        private Rigidbody _rb;
        private Animator _animator;

        private Vector2 _moveInput; // x = gauche/droite, y = avant/arrière
        private bool _movementEnabled = true;
        private float _externalSpeedModifier = 1f;
        private bool _isSprinting;
        private bool _isWalking;

        // Valeurs anim interpolées (pour remplacer un BlendTree 2D par un Lerp côté code)
        private float _animMoveX;
        private float _animMoveY;

        public Vector2 MoveInput => _moveInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            // On freeze maintenant X, Y et Z pour empêcher toute rotation physique
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        public void SetMoveInput(Vector2 input)
        {
            _moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        public void IsSprinting(bool isSprinting )
        {
            _isSprinting = isSprinting;
        }

        public void IsWalking(bool isWalking)
        {
            _isWalking = isWalking;
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

            float finalSpeed = baseMoveSpeed * speedFactor * _externalSpeedModifier;

            Vector3 moveDir = new Vector3(input.x, 0f, input.y);

            // Mouvement physique
            Vector3 targetVelocity = moveDir.normalized * finalSpeed;
            targetVelocity.y = _rb.linearVelocity.y; 
            _rb.linearVelocity = targetVelocity;

            // Rotation du personnage vers la direction de déplacement
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }


        private void UpdateAnimatorParameters()
        {
            if (_animator == null) return;

            _animator.SetBool("IsSprinting", _isSprinting);
            _animator.SetBool("IsWalking", _isWalking);
        }
    }
}
