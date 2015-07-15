using UnityEngine;
using System.Collections;
using Prime31;

public class PlayerControl : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float knockbackHeight = 1f;
	public bool becameKnockedBackThisFrame = false;
	public bool wasKnockedBackLastFrame = false;
	public bool IFrames = false;
	[HideInInspector]
	private float
		normalizedHorizontalSpeed = 0;
	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private CollisionManager _colManager;
	//private LayerMask damageMask;

	void Awake ()
	{
		_animator = GetComponent<Animator> ();
		_controller = GetComponent<CharacterController2D> ();
		_colManager = GetComponent<CollisionManager> ();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		 
	}


	#region Event Listeners

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

		if (col.CompareTag ("Actor")) {
			Debug.Log ("Collider Tag: " + col.tag);
			if(1<<col.gameObject.layer == LayerMask.GetMask("Hitbox"))
				_colManager.Collision(col);
		}
		if (col.CompareTag ("Prop")) {
			//_colManager.Collision (col);
		}
		if (col.CompareTag ("Terrain")) {
			//_colManager.Collision (col);
		}
	}

	void onTriggerExitEvent (Collider2D col)
	{
		Debug.Log ("onTriggerExitEvent: " + col.gameObject.name);
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update ()
	{
		if (_controller.isGrounded)
			_velocity.y = 0;

		// If Player became knockedback
		if (becameKnockedBackThisFrame) {
			// Adjust player velocity
			normalizedHorizontalSpeed = Mathf.Sign (_velocity.x) * -0.5f;
			_velocity.y = Mathf.Sqrt (2f * knockbackHeight * -gravity);
			_animator.Play( Animator.StringToHash( "PLAYER_AIRBORNE" ) );

			// And set variables to halt input
			becameKnockedBackThisFrame = false;
			wasKnockedBackLastFrame = true;
		} else if (wasKnockedBackLastFrame) {
			// If the player was knockedback, but has landed they stop being knocked back.
			if (_controller.isGrounded) {
				wasKnockedBackLastFrame = false;
			}
			// Else, do nothing and accept no input
		}

		if (!wasKnockedBackLastFrame && !becameKnockedBackThisFrame) {
			if (Input.GetKey (KeyCode.RightArrow)) {
				normalizedHorizontalSpeed = 1;
				if (transform.localScale.x < 0f)
					transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

				if (_controller.isGrounded)
					_animator.Play (Animator.StringToHash ("PLAYER_WALK"));
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				normalizedHorizontalSpeed = -1;
				if (transform.localScale.x > 0f)
					transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

				if (_controller.isGrounded)
					_animator.Play (Animator.StringToHash ("PLAYER_WALK"));
			} else {
				normalizedHorizontalSpeed = 0;

				if (_controller.isGrounded)
					_animator.Play (Animator.StringToHash ("PLAYER_IDLE"));
			}

			// we can only jump whilst grounded and are not knocked back
			if (_controller.isGrounded && Input.GetKeyDown (KeyCode.UpArrow)) {
				_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
				_animator.Play (Animator.StringToHash ("PLAYER_AIRBORNE"));
			}
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp (_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets uf jump down through one way platforms
		if (_controller.isGrounded && Input.GetKey (KeyCode.DownArrow) ) {
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}


		_controller.move (_velocity * Time.deltaTime);

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

	// Apply a knockback effect based on the vector between the collision point and the objects
	// Transform position.
	public void Knockback()
	{
		Debug.Log ("Knockback called");
		becameKnockedBackThisFrame = true;
	}
}
