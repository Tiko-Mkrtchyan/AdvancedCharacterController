using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private float walkSpeed = 3;
        [SerializeField] private float rotationSpeed = 500f;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float jumpForce;

        private Vector2 _moveInput;
        private float _ySpeed;
        private Quaternion _targetRotation;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private Vector3 _moveDirection;
        private bool IsGrounded() => characterController.isGrounded;

        private void Awake()
        {
            _moveAction = new PlayerInputActions().Player.Move;
            _jumpAction = new PlayerInputActions().Player.Jump;
            _jumpAction.performed += HandleJump;
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _jumpAction.Enable();
        }

        private void Update()
        {
            if (_ySpeed <= 0.5f)
            {
                playerAnimator.SetBool("jump", false);
            }

            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            _moveInput = _moveAction.ReadValue<Vector2>().normalized;
            _moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
            _moveDirection = cameraController.Rotation * _moveDirection;

            var velocity = _moveDirection * walkSpeed;
            velocity.y = _ySpeed;

            characterController.Move(velocity * Time.deltaTime);

            if (IsGrounded())
            {
                _ySpeed = -0.5f;
                playerAnimator.SetBool("falen", true);
                playerAnimator.SetBool("runToJump",false);
            }
            else
            {
                _ySpeed += Physics.gravity.y * Time.deltaTime;
            }

            if (IsGrounded() && playerAnimator.GetBool("jump"))
            {
                playerAnimator.SetBool("jump", false);
            }

            playerAnimator.SetFloat("moveAmount", _moveDirection.magnitude, 0.2f, Time.deltaTime);
        }

        private void HandleRotation()
        {
            if (_moveInput != Vector2.zero)
            {
                _targetRotation =
                    Quaternion.LookRotation(cameraController.Rotation * new Vector3(_moveInput.x, 0, _moveInput.y));
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void HandleJump(InputAction.CallbackContext context)
        {
            if (IsGrounded() && _moveDirection.magnitude == 0)
            {
                playerAnimator.SetBool("jump", true);
                playerAnimator.SetBool("falen", false);
                _ySpeed = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }

            if (_moveDirection.magnitude > 0 && IsGrounded())
            {
                playerAnimator.SetBool("runToJump", true);
                playerAnimator.SetBool("fallen", false);
                _ySpeed = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _jumpAction.Disable();
        }
    }
}