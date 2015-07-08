using UnityEngine;
using System.Collections;
//
// Script handling the basic state machine for an Actor class object.
//
public class ActorController : MonoBehaviour {
/*
	// Heavily modified from deranged-hermit.blogspot.com

	// Basic Physics properties, all in units/second
	float acceleration = 4f;
	float maxSpeed = 150f;
	float gravity = 6f;
	float maxFall = 200f;
	float jump = 200f;

	// layerMasks
	int layerMask;
	int platformMask;
	int colliderMask;

	// Rectangle class obj to save coding
	Rect box;

	// 2D velocity variable
	Vector2 velocity;

	// Checks
	bool grounded;
	bool falling;

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
	private float h;
	public float setHorizontal {
		set{ h = value;}
	}

	protected float walkSpeed = 2f;
	protected float runSpeed = 4f;
	protected float moveForce = 10f;
	protected float maxIncline = 45f;




	// Use this for initialization
	void Start () {
		// Mask for objects you can stand on
		platformMask = LayerMask.GetMask(new String[]{"Terrain", "Actor"});

		rb2d = gameObject.GetComponent<Rigidbody2D> ();
		anim = gameObject.GetComponent<Animator> ();
		sprite = gameObject.GetComponent<SpriteRenderer> ();
		bCol2D = gameObject.GetComponent<BoxCollider2D> ();
		facingRight = true;
		grounded = true;
		h = 0;

	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
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


		// update box to current bound position
		box = new Rect (
			bCol2D.bounds.min.x,
			bCol2D.bounds.min.y,
			bCol2D.bounds.max.x,
			bCol2D.bounds.max.y
		);

		Gravity ();

		//Update horizontal velocity
		velocity = new Vector2 (Mathf.Min(velocity.x + maxSpeed * Time.deltaTime, maxSpeed), velocity.y);

	}

	void LateUpdate(){
		transform.Translate (velocity * Time.deltaTime);
	}


	// Function that causes the Actor to jump if allowed
	// Criss-cross make you wanna..
	public void Jump()
	{
		if (grounded) {
			Debug.Log ("JUMP");
			velocity = new Vector2(velocity.x, velocity.y + jump);
		}
	}

	// Function that changes the Sprite's facing
	public void Flip()
	{
		Debug.Log ("Flip called");
		// Switch the way the Player is labeled as facing
		facingRight = !facingRight;
		// Switch the sprites X-axis
		Vector3 t = transform.localScale;
		t.x *= -1;
		transform.localScale = t;
	}

	public void Move(float h, int mType = 1){

		if (mType == 1) {
			// Add horizontal force
			if (Mathf.Abs (rb2d.velocity.x) < walkSpeed)
				rb2d.velocity = new Vector2 (h * walkSpeed, rb2d.velocity.y);
			// Limit setHorizontal velocity to walkSpeed
			if (Mathf.Abs (rb2d.velocity.x) > walkSpeed)
				rb2d.velocity = new Vector2 (Mathf.Sign (h) * walkSpeed, rb2d.velocity.y);
		}

	}

	public void Attack(){
		anim.SetTrigger("Attack");
	}

	public void Gravity(){
		if (!grounded)
			velocity = new Vector2 (velocity.x, Mathf.Max (velocity.y - gravity, -maxFall));
		
		if (velocity.y < 0) {
			falling = true;
		}
		
		if (grounded || falling){
			Vector3 startPoint = new Vector3(box.xMin + margin, box.center.y, transform.position.z);
			Vector3 endPoint = new Vector3(box.xMax - margin, box.center.y, transform.position.z);
			
			RaycastHit2D hitInfo;
			
			// add half my box height since it starts from the center
			float distance = box.height/2 + (grounded? margin: Mathf.Abs (velocity.y * Time.deltaTime));
			
			// Check if you hit anything
			bool connected = false;
			
			for(int i = 0; i < verticalRays; i++){
				
				float lerpAmount = (float)i / (float)verticalRays - 1;
				Vector3 origin = Vector3.Lerp (startPoint, endPoint, lerpAmount);
				Ray ray = new Ray(origin, Vector3.down);
				
				connected = Physics.Raycast (ray, out hitInfo, platformMask);
				
				if(connected){
					grounded = true;
					falling = false;
					transform.Translate(Vector3.down * (hitInfo.distance - box.height/2));
					velocity = new Vector2(velocity.x, 0);
					break;
				}
			}
			
			if(!connected){
				grounded = false;
			}
			
		}
	}

	protected bool isGrounded {
		get{
			bool result = false;
			Debug.Log ("isGrounded Called");
			LayerMask mask = LayerMask.GetMask (new string[] {"Terrain","Actor"});
			if (bCol2D.IsTouchingLayers (mask)) {
				// Boxcast down to nearest Terrain or Actor object
				RaycastHit2D bHit = Physics2D.BoxCast (bCol2D.bounds.center, bCol2D.bounds.size.x, Vector2.down, Mathf.Infinity, mask);
				//Debug.DrawRay(transform.position, Vector3.down, Color.green);
				// Draw the Raycast for Debug purposes
				// Debug.DrawRay (transform.position, Vector3.down, Color.green);

				// If Boxcast hit something
				if (bHit) {
					// ... Raycast down.
					RaycastHit2D rHit = Physics2D.Raycast (bCol2D.bounds.center, Vector3.down, Mathf.Infinity, mask);
					// If Raycast hit something in range of bHit...
					if(rHit){
						// Check if the incline is flat enough...
						if (Vector2.Angle (rHit.point, bHit.point) <= maxIncline) {
							// ... and set result if object hit is a "Platform"
							result = bHit.collider.CompareTag ("Platform");
						}
					}
				}
			}
			return result;
		}
	}*/
}
