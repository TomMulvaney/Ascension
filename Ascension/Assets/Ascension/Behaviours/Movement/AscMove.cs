using UnityEngine;
using System.Collections;
using Prime31;

public class AscMove : MonoBehaviour
{
    public readonly string VERTICAL = "Vertical";
    public readonly string HORIZONTAL = "Horizontal";
    public readonly string RUN = "Run";
    public readonly string JUMP = "Jump";
    
    // movement config
    public float gravity = 20f;
    public float runSpeed = 8f;
    public float groundDamping = 20f;
    // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public bool useSingleDamping = true;
    public float jumpHeight = 3f;
    public int maxAirJumps = 1;
    public float hookshotSpeed = 100f;
    public float wallSlideSpeed = 1f;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;

    private IHookshot _hookshot;
    private Rigidbody2D _rb;

    int airJumps = 0;

    bool displayGUI = true;
    
    float wallExitJumpThreshold = 0.05f;


    void Awake ()
    {
        _animator = GetComponent<Animator> ();
        _controller = GetComponent<CharacterController2D> ();

        _hookshot = GetComponent<IHookshot> ();
        _rb = GetComponent<Rigidbody2D> ();

        Debug.Log (_hookshot);

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }


    #region Event Listeners
    
    float smoothedMovementFactor
    {
        get {
            if (useSingleDamping) {
                return groundDamping;
            } else {
                return _controller.isGrounded ? groundDamping : inAirDamping; 
            }
        }
    }

    void onControllerCollider (RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent (Collider2D col)
    {
        Debug.Log ("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent (Collider2D col)
    {
        Debug.Log ("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion

    bool Jump ()
    {
        if (Input.GetButtonDown (JUMP)) {
            _velocity.y = Mathf.Sqrt (2f * jumpHeight * gravity);
            _animator.Play (Animator.StringToHash (JUMP));
            return true;
        } else {
            return false;
        }
    }

    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update ()
    {
        // Reset
        // Find horizontal
        // Jump
        // Apply horizontal
        // Gravity
        // Platform drop
        // Move
        int normHorizontalMove = CalcNormHorizontalMove ();         
        
        if (_controller.isGrounded) {
            WalkAnim (normHorizontalMove);
            airJumps = 0;
            _velocity.y = 0;
            Jump ();
            _velocity.x = Mathf.Lerp (_velocity.x, normHorizontalMove * runSpeed, Time.deltaTime * smoothedMovementFactor);
            
            // if holding down bump up our movement amount and turn off one way platform detection for a frame.
            // this lets uf jump down through one way platforms
            if (Input.GetAxis (VERTICAL) < -0.9f) {
                _velocity.y *= 3f;
                _controller.ignoreOneWayPlatformsThisFrame = true;
            }
        } else if (_controller.collisionState.left || _controller.collisionState.right) {
            airJumps = 0;
            _velocity.y = -wallSlideSpeed;
            Jump ();
            
            if ((normHorizontalMove == -1 && _controller.collisionState.left) || 
                (normHorizontalMove == 1 && _controller.collisionState.right)) {
                _velocity.x = _controller.collisionState.left ? -0.1f : 0.1f;
            } else {
                _velocity.x = Mathf.Lerp (_velocity.x, normHorizontalMove * runSpeed, Time.deltaTime * smoothedMovementFactor);
            }
        } else {
            WalkAnim (normHorizontalMove);
            if (airJumps < maxAirJumps) {
                bool hasJumped = Jump ();
                if (hasJumped) {
                    Debug.Log ("Has Jumped");
                    airJumps += 1;
                }
            }
            
            if (normHorizontalMove != 0 && (Mathf.Abs (_velocity.x) <= runSpeed || _velocity.x * normHorizontalMove < 0)) {
                _velocity.x = Mathf.Lerp (_velocity.x, normHorizontalMove * runSpeed, Time.deltaTime * inAirDamping);   
            }
        } 

        // apply gravity before moving
        _velocity.y -= gravity * Time.deltaTime;

        Vector2 hookshotVec = _hookshot.GetHookshotVector (transform.position);
        
        hookshotVec *= hookshotSpeed;
        if (!Mathf.Approximately (hookshotVec.magnitude, 0)) {
            _velocity.x = hookshotVec.x;
            _velocity.y = hookshotVec.y;
        }

        _controller.move (_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
    
    void WalkAnim (int normHorizontalMove)
    {
        if (normHorizontalMove == 1) {
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
                _animator.Play (Animator.StringToHash ("Run"));
        } else if (normHorizontalMove == -1) {
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
                _animator.Play (Animator.StringToHash ("Run"));
        } else {
            if (_controller.isGrounded)
                _animator.Play (Animator.StringToHash ("Idle"));
        }
    }

    int CalcNormHorizontalMove ()
    {
        float threshold = 0.75f;
        float horizontal = Input.GetAxis (HORIZONTAL);
        
        if (horizontal > threshold) {
            return 1;
        } else if (horizontal < -threshold) {
            return -1;
        } else {
            return 0;
        }
    }

    void OnGUI ()
    {
        if (displayGUI) {
            GUILayout.Label (string.Format ("airJump: {0} / {1}", airJumps, maxAirJumps));
//            GUILayout.Label (string.Format ("xVelocity: {0}", _velocity.x));
//            GUILayout.Label (string.Format ("yVelocity: {0}", _velocity.y));
//            GUILayout.Label (string.Format ("normHoriSpeed: {0}", normHorizontalMove));
//            GUILayout.Label (string.Format ("horizontal: {0}", Input.GetAxis (HORIZONTAL)));
            GUILayout.Label (string.Format ("left: {0}", _controller.collisionState.left));
            GUILayout.Label (string.Format ("right: {0}", _controller.collisionState.right));
        }
    }
}