using UnityEngine;
using System.Collections;

//
// Script handling the basic state machine for an Actor class object.
//
public class ActorController2D : MonoBehaviour
{

	// Heavily modified from deranged-hermit.blogspot.com

	// Basic Physics properties, all in units/second
	public float acceleration = 4f;
	public float maxSpeed = 150f;
	public float gravity = 6f;
	public float maxFall = 200f;
	public float jump = 200f;
	public float maxIncline = 5f;

	// layerMasks
	int layerMask;
	int platformMask;
	int colliderMask;

	// Rectangle class obj to save coding
	Rect box;

	// 2D velocity variable
	public Vector2 velocity;

	// Checks
	public bool grounded;
	public bool falling;

	// Raycast variables
	int horizontalRays = 6;
	int verticalRays = 4;
	int margin = 2;

	//My Stuff

	protected Animator anim;
	protected Rigidbody2D rb2d;
	protected SpriteRenderer sprite;
	protected BoxCollider2D bCol2D;
	protected bool facingRight;

	// Horizontal factor (-1 to 1)
	public float horizontalAxis;
	protected float walkSpeed = 2f;
	protected float runSpeed = 4f;
	protected float moveForce = 10f;





	// Use this for initialization
	void Start ()
	{
		// Mask for objects you can stand on
		platformMask = LayerMask.GetMask (new string[]{"Terrain", "Actor"});

		rb2d = gameObject.GetComponentInChildren<Rigidbody2D> ();
		anim = gameObject.GetComponentInChildren<Animator> ();
		sprite = gameObject.GetComponentInChildren<SpriteRenderer> ();
		bCol2D = gameObject.GetComponentInChildren<BoxCollider2D> ();
		facingRight = true;
		falling = false;
		grounded = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		velocity = rb2d.velocity;
		
		// Update Animation variables
		anim.SetBool("Grounded", grounded);
		//Debug.Log ("Grounded set " + grounded);
		
		anim.SetFloat ("Velocity", Mathf.Abs (horizontalAxis));
		
		if ((facingRight && horizontalAxis < 0) || (!facingRight && horizontalAxis > 0)) {
			Debug.Log ("Flip called");
			Flip ();
		}

	}

	void FixedUpdate ()
	{
		/* Check player facing
		if (h < 0 && facingRight) {
			Debug.Log ("Moving Left");
			Flip ();
		}
		if (h > 0 && !facingRight) {
			Debug.Log ("Moving Right");
			Flip ();
		}
		
		// If player's motion is contrary to last known grounded state, check grounded state
		if ((Mathf.Abs(rb2d.velocity.y) <= 1 && !grounded)||(Mathf.Abs(rb2d.velocity.y) > 1 && grounded)) {
			Debug.Log ("Grounded Check");
			grounded = isGrounded;
		}

		// Update grounded Animation State
		anim.SetBool ("Grounded", grounded);
		*/

		// update box to current bound position
		box = new Rect (
			bCol2D.bounds.min.x,
			bCol2D.bounds.min.y,
			bCol2D.bounds.max.x,
			bCol2D.bounds.max.y
		);

		//---------------------------------------------------------------------------------------\\
		//-------------------------------Vertical Movement---------------------------------------\\
		//---------------------------------------------------------------------------------------\\
		Gravity ();



		//---------------------------------------------------------------------------------------\\
		//-------------------------------Lateral Movement---------------------------------------\\
		//---------------------------------------------------------------------------------------\\
		Move ();




	}

	void LateUpdate ()
	{
		transform.Translate (velocity * Time.deltaTime);
	}


	// Function that causes the Actor to jump if allowed
	// Criss-cross make you wanna..
	public void Jump ()
	{
		if (grounded) {
			Debug.Log ("JUMP");
			velocity = new Vector2 (velocity.x, jump);
			grounded = false;
			//Debug.Log ("Grounded set " + grounded + "(false)");
		}
	}
	
	public void Move ()
	{

		float newVelocityX = velocity.x;
		if (horizontalAxis != 0) { // accelerate based on input
			newVelocityX += acceleration * horizontalAxis;
			newVelocityX = Mathf.Clamp (newVelocityX, -maxSpeed, maxSpeed);
		} else if (velocity.x != 0) { // apply deceleration from no input
			float modifier = -(Mathf.Sign (velocity.x));
			newVelocityX += acceleration * modifier;
			if (Mathf.Sign (newVelocityX) == Mathf.Sign (modifier))
				newVelocityX = 0;
		}

		velocity = new Vector2 (newVelocityX, velocity.y);

		if (velocity.x != 0) { // do physics checks if i'm going to move
			Vector2 startPoint = new Vector2 (box.center.x, box.yMin /*+margin*/);
			Vector2 endPoint = new Vector2 (box.center.x, box.yMax /*-margin*/);

			RaycastHit2D[] hitInfos = new RaycastHit2D[horizontalRays];
			int amountConnected = 0;
			float lastFraction = 0;

			float sideRayLength = (box.width / 2) + Mathf.Abs (newVelocityX * Time.deltaTime);
			Vector2 direction = (newVelocityX > 0) ? Vector2.right : -Vector2.right;
			bool connected = false;

			for (int i = 0; i < horizontalRays; i++) { // Go through all the rays
				float lerpAmount = (float)i / (float)(horizontalRays - 1);
				Vector2 origin = Vector2.Lerp (startPoint, endPoint, lerpAmount);
				//Ray ray = new Ray (origin, direction);

				//Did I connect with the thing?
				hitInfos [i] = Physics2D.Raycast (origin, direction, sideRayLength, platformMask);

				if (hitInfos [i].fraction > 0 && hitInfos[i].collider.CompareTag("Platform")) {
					connected = true;
					if (lastFraction > 0) {
						float angle = Vector2.Angle (hitInfos [i].point - hitInfos [i - 1].point, Vector2.right);

						if (Mathf.Abs (angle /*- 90*/) < maxIncline) {
							transform.Translate (direction * (hitInfos [i].distance - box.width / 2));
							velocity = new Vector2 (0, velocity.y);
							break;
						}
					}
					amountConnected++;
					lastFraction = hitInfos [i].fraction;
				}
			}

		}
	}
	
	public void Gravity ()
	{
		if (!grounded) {
			velocity = new Vector2 (velocity.x, Mathf.Max (velocity.y - gravity, -maxFall));
		}
		
		if (velocity.y < 0) {
			falling = true;
		}
		
		if (grounded || falling) {
			Vector2 startPoint = new Vector2 (box.xMin /*+ margin*/, box.center.y);
			Vector2 endPoint = new Vector2 (box.xMax /*- margin*/, box.center.y);
			RaycastHit2D[] hitInfos = new RaycastHit2D[verticalRays];
			
			// add half my box height since it starts from the center
			float distance = box.height / 2 + (grounded ? margin : Mathf.Abs (velocity.y * Time.deltaTime));

			float smallestFraction = Mathf.Infinity;
			int indexUsed = 0;

			// Check if you hit anything
			bool connected = false;
			
			for (int i = 0; i < verticalRays; i++) {
				
				float lerpAmount = (float)i / (float)(verticalRays - 1);
				Vector2 origin = Vector2.Lerp (startPoint, endPoint, lerpAmount);
				//Ray ray = new Ray (origin, Vector2.down);
				
				hitInfos [i] = Physics2D.Raycast (origin, Vector2.down, distance, platformMask);

				if (hitInfos [i].fraction > 0 && hitInfos[i].collider.CompareTag("Platform")) {
					connected = true;
					if (hitInfos [i].fraction < smallestFraction) {

						indexUsed = i;
						smallestFraction = hitInfos [i].fraction;
					}
				}
			}

			if (connected) {
				Debug.Log ("Gravity Raycast.fraction: " + hitInfos[indexUsed].fraction);
				grounded = true;
				falling = false;
				transform.Translate (Vector3.down * (hitInfos [indexUsed].distance - box.height / 2));
				velocity = new Vector2 (velocity.x, 0);
			} else {
				grounded = false;
			}
		}
	}

	// Function that changes the Sprite's facing
	public void Flip ()
	{
		Debug.Log ("Flip called");
		// Switch the way the Player is labeled as facing
		facingRight = !facingRight;
		// Switch the sprites X-axis
		Vector3 t = transform.localScale;
		t.x *= -1;
		transform.localScale = t;
	}

	public void Attack ()
	{
		anim.SetTrigger ("Attack");
	}

}
