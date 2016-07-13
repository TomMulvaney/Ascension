using UnityEngine;
using System.Collections;
using Prime31;

public class AscMove : MonoBehaviour
{
	// movement config
	[SerializeField]
	private float gravity = -25f;
	[SerializeField]
	private float runSpeed = 8f;
	[SerializeField]
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	[SerializeField]
	private float inAirDamping = 5f;
	[SerializeField]
	private float groundAcceleration = 20f;
	[SerializeField]
	private float groundDecceleration = 20f;
	[SerializeField]
	private float airAcceleration = 5f;
	[SerializeField]
	private float airDecceleration = 0f;
	[SerializeField]
	private float jumpHeight = 3f;
	[SerializeField]
	private int airJumps = 1;
	[SerializeField]
	private float hookshotSpeed = 100f;


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


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		if (_controller.isGrounded) {
			airJumpCount = 0;
			_velocity.y = 0;
		}

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


		// we can only jump whilst grounded
		if((_controller.isGrounded  || airJumpCount < airJumps) && Input.GetKeyDown( KeyCode.UpArrow ) )
		{
			airJumpCount = _controller.isGrounded ? 0 : airJumpCount + 1;
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
		}


		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );



		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets uf jump down through one way platforms
		if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		Vector3 hookshotVec = _hookshot.GetHookshotVector (transform.position) * hookshotSpeed;
		if(!Mathf.Approximately(hookshotVec.magnitude, 0)) {
			Debug.Log(hookshotVec.magnitude);
			Debug.Log(hookshotVec);
		}

		_velocity.x += hookshotVec.x;
		_velocity.y += hookshotVec.y;

		_controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

	void OnGUI() {
		if (displayGUI) {
			GUILayout.Label (string.Format ("AirJumpCount: {0} / {1}", airJumpCount, airJumps));
		}
	}
}