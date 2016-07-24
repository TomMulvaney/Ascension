using UnityEngine;
using System.Collections;
using Prime31;

public class AscMove : MonoBehaviour
{
    // movement config
    public float gravity = 20f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
    public int airJumps = 1;
    public float hookshotSpeed = 100f;
    public float wallSlideSpeed = 1f;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	private IHookshot _hookshot;
	private Rigidbody2D _rb;

	int airJumpCount = 1;

	bool displayGUI = true;


	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		_hookshot = GetComponent<IHookshot> ();
		_rb = GetComponent<Rigidbody2D> ();

		Debug.Log (_hookshot);

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion

	bool Jump()
    {
        if(Input.GetKeyDown( KeyCode.UpArrow ))
        {
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
			return true;
		}
        else
        {
			return false;
		}
	}
    
    void CalcNormalizedHorizontalMovement()
    {
        if( Input.GetKey( KeyCode.RightArrow ) )
        {
            normalizedHorizontalSpeed = 1;
            if( transform.localScale.x < 0f )
                transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

            if( _controller.isGrounded )
                _animator.Play( Animator.StringToHash( "Run" ) );
        }
        else if( Input.GetKey( KeyCode.LeftArrow ) )
        {
            normalizedHorizontalSpeed = -1;
            if( transform.localScale.x > 0f )
                transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

            if( _controller.isGrounded )
                _animator.Play( Animator.StringToHash( "Run" ) );
        }
        else
        {
            normalizedHorizontalSpeed = 0;

            if( _controller.isGrounded )
                _animator.Play( Animator.StringToHash( "Idle" ) );
        }
    }


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        // Reset
        // Find horizontal
        // Jump
        // Apply horizontal
        // Gravity
        // Platform drop
        // Move
        
        if (_controller.isGrounded) {
            airJumpCount = 0;
            _velocity.y = 0;
            CalcNormalizedHorizontalMovement ();
            Jump ();
            _velocity.x = Mathf.Lerp (_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * groundDamping);
            
            // if holding down bump up our movement amount and turn off one way platform detection for a frame.
            // this lets uf jump down through one way platforms
            if (Input.GetKey (KeyCode.DownArrow)) {
                _velocity.y *= 3f;
                _controller.ignoreOneWayPlatformsThisFrame = true;
            }
        } else if (_controller.collisionState.left || _controller.collisionState.right) {
            airJumpCount = 0;
            _velocity.y = -wallSlideSpeed;
            Jump ();
        }
        else 
        {
            CalcNormalizedHorizontalMovement ();
            if(airJumpCount < airJumps)
            {
                airJumpCount += Jump () ? 1 : 0;
            }
            
            if (normalizedHorizontalSpeed != 0 && (Mathf.Abs (_velocity.x) <= runSpeed || _velocity.x * normalizedHorizontalSpeed < 0)) 
            {
                _velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * inAirDamping );   
            }
		} 

		// apply gravity before moving
		_velocity.y -= gravity * Time.deltaTime;

		
		Vector2 hookshotVec = _hookshot.GetHookshotVector (transform.position);
        
        
        hookshotVec *= hookshotSpeed;
		if(!Mathf.Approximately(hookshotVec.magnitude, 0)) {
            _velocity.x = hookshotVec.x;
            _velocity.y = hookshotVec.y;
		}

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

	void OnGUI() {
		if (displayGUI) {
			GUILayout.Label (string.Format ("AirJumpCount: {0} / {1}", airJumpCount, airJumps));
            GUILayout.Label (string.Format ("VelocityX: {0}", _velocity.x));
            GUILayout.Label (string.Format ("VelocityY: {0}", _velocity.y));
		}
	}
}