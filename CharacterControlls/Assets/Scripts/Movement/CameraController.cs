using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float thirdPersonMinYAngle = -80f;
        [SerializeField] private float thirdPersonMaxYAngle = 80f;

        [SerializeField] private float firstPersonMinYAngle = -50f;
        [SerializeField] private float firstPersonMaxYAngle = 60f;

        [SerializeField] private float distanceZ = 4f;
        [SerializeField] private Vector3 firstPersonOffset;
        [Range(0, 5)] [SerializeField] private float sensitivity = 1.3f;

        private float _currentXRotation = 0;
        private float _rotationY;
        private InputAction _lookAction;
        private InputAction _switchViewAction;
        private bool _isFirstPerson = false;

        public Quaternion Rotation => Quaternion.Euler(0, _rotationY, 0);

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CheckView();
            var inputActions = new PlayerInputActions();
            _lookAction = inputActions.Player.Look;
            _switchViewAction = inputActions.Player.ChangeView;
        }

        private void OnEnable()
        {
            _lookAction.Enable();
            _switchViewAction.Enable();

            _switchViewAction.performed += _ => ToggleView();
        }

        private void OnDisable()
        {
            _lookAction.Disable();
            _switchViewAction.Disable();
        }

        private void Update()
        {
            HandleCameraMovement();
        }

        private void HandleCameraMovement()
        {
            var lookInput = _lookAction.ReadValue<Vector2>();

            var mouseX = lookInput.y * sensitivity;
            var mouseY = lookInput.x * sensitivity;

            _rotationY += mouseY;

            if (_isFirstPerson)
            {
                _currentXRotation -= mouseX;
                _currentXRotation = Mathf.Clamp(_currentXRotation, firstPersonMinYAngle, firstPersonMaxYAngle);

                transform.position = followTarget.position + firstPersonOffset;
                transform.rotation = Quaternion.Euler(_currentXRotation, _rotationY, 0);
            }
            else
            {
                _currentXRotation -= mouseX;
                _currentXRotation = Mathf.Clamp(_currentXRotation, thirdPersonMinYAngle, thirdPersonMaxYAngle);

                var targetRotation = Quaternion.Euler(_currentXRotation, _rotationY, 0);
                transform.position = followTarget.position - targetRotation * new Vector3(0, 0, distanceZ);
                transform.rotation = targetRotation;
            }
        }

        private void ToggleView()
        {
            _isFirstPerson = !_isFirstPerson;
            CheckView();
        }

        private void CheckView()
        {
            if (_isFirstPerson)
            {
                _currentXRotation = transform.eulerAngles.x;
                _rotationY = followTarget.eulerAngles.y;

                transform.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
            }
            else
            {
                transform.GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Player"));
            }
        }
    }
}