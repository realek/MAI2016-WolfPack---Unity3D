using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    
    public float PlayerSpeed = 2.5f;
    public float SmoothStart = 0.1f;
    public float _currentSpeed = 0f;
    public bool allowCamMovement = true;
    public bool allowBodyMovement = true;
    public bool allowCollide = true;
    [SerializeField] private MouseLook MouseLook = new MouseLook();
    [SerializeField] private float _GravityMultiplier = 1;
    [SerializeField] private float _JumpSpeed = 4;
    [SerializeField] private float _StickToGroundForce = 10;

    private Camera _Camera;
    private CharacterController _CharacterController;
    private Vector3 _MoveDir = Vector3.zero;
    private CollisionFlags _CollisionFlags;
    private Vector2 _Input;
    private bool _isStill;
    private bool _pressedJump;

    void Awake() {
        _currentSpeed = 0f;
        _CharacterController = GetComponent<CharacterController>();
        _Camera = Camera.main;
        MouseLook.Init(transform, _Camera.transform);
        _pressedJump = false;
    }

    void Update() {
        if (allowCamMovement) RotateView();
        else ResetMouseLook();

        if (!_pressedJump) {
            _pressedJump = Input.GetButtonDown("Jump");
        }
    }

    public void ResetMouseLook() {
        MouseLook.Init(transform, _Camera.transform);
    }

    private void FixedUpdate() {
        if (allowBodyMovement) {
            GetInput();
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * _Input.y + transform.right * _Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, _CharacterController.radius, Vector3.down, out hitInfo,
                _CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            // horizontal movement
            _isStill = (desiredMove.x * desiredMove.x < 0.0001f && desiredMove.z * desiredMove.z < 0.0001f);
            if (_isStill) {
                _currentSpeed = 0;
                _MoveDir.x = 0;
                _MoveDir.z = 0;
            } else if (!_isStill && _currentSpeed < PlayerSpeed - 0.01) {
                _currentSpeed += SmoothStart;
                if (_currentSpeed > PlayerSpeed) _currentSpeed = PlayerSpeed;
                _MoveDir.x = desiredMove.x * _currentSpeed;
                _MoveDir.z = desiredMove.z * _currentSpeed;
            } else {
                _MoveDir.x = desiredMove.x * PlayerSpeed;
                _MoveDir.z = desiredMove.z * PlayerSpeed;
            }

            // vertical movement
            if (_CharacterController.isGrounded) {
                _MoveDir.y = -_StickToGroundForce;

                if (_pressedJump) {
                    Debug.Log("Jumped");
                    _MoveDir.y = _JumpSpeed;
                    _pressedJump = false;
                }
            } else {
                _MoveDir += Physics.gravity * _GravityMultiplier * Time.fixedDeltaTime;
            }

            _CollisionFlags = _CharacterController.Move(_MoveDir * Time.fixedDeltaTime);
        }
    }

    private void RotateView() {
        MouseLook.LookRotation(transform, _Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (allowCollide) {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (_CollisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }

    private void GetInput() {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (_Input.sqrMagnitude > 1) {
            _Input.Normalize();
        }
    }
}
