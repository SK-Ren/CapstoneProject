using UnityEngine;
using System.Collections;
//
// Script handling the basic state machine for an Actor class object.
//
public class ActorController : MonoBehaviour {

	/// Basic Physics properties, all in units/second
	public float acceleration = 3f;
	public float maxSpeed = 5f;
	public float gravity = 1f;
	public float maxFall = 10f;
	public float jumpForce = 3f;
	public float maxIncline = 5f;

	public Vector2 velocity;

	
	// layerMasks
	int layerMask;
	int platformMask;
	int colliderMask;
	
	// Rectangle class obj to save coding
	Rect box;
	
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
	void Update () {
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

	void FixedUpdate(){
		// update box to current bound position
		box = new Rect (
			bCol2D.bounds.min.x,
			bCol2D.bounds.min.y,
			bCol2D.bounds.max.x,
			bCol2D.bounds.max.y
		);
		Move ();
	}

	void LateUpdate(){
		//rb2d.velocity = velocity;
	}


	// Function that causes the Actor to jump if allowed
	// Criss-cross make you wanna..
	public void Jump()
	{
		if (grounded) {
			Debug.Log ("JUMP");
			rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			grounded = false;
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

	public void Move(){

		if (rb2d.velocity.y < 0 && !grounded) {
			falling = true;
		}

		// ---------------------------------------------------\\
		//------------Horizontal Force------------------------\\
		//----------------------------------------------------\\

		if (horizontalAxis != 0) {
			velocity = new Vector2 (Mathf.Clamp (rb2d.velocity.x + (horizontalAxis * acceleration), -maxSpeed, maxSpeed), rb2d.velocity.y);
		} else if (rb2d.velocity.x != 0) { // apply deceleration from no input
			float modifier = -(Mathf.Sign (rb2d.velocity.x));
			rb2d.velocity = new Vector2( rb2d.velocity.x - (acceleration * modifier), rb2d.velocity.y);
			if (Mathf.Sign (rb2d.velocity.x) == Mathf.Sign (modifier))
				rb2d.velocity = new Vector2( 0, rb2d.velocity.y);
		}

		if (bCol2D.IsTouchingLayers (platformMask)) {
			
			// Vectors of HitBox bounds, Empty RaycastHit2D array
			Vector2 startPoint = new Vector2 (box.xMin /*+ margin*/, box.center.y);
			Vector2 endPoint = new Vector2 (box.xMax /*- margin*/, box.center.y);
			RaycastHit2D[] hitInfos = new RaycastHit2D[verticalRays];
			
			// Distance for raycast to go down.
			float distance = box.height / 2 + (grounded ? margin : Mathf.Abs (rb2d.velocity.y * Time.deltaTime));
			int amountConnected = 0;
			float lastFraction = 0;
			
			bool connected = false;
			
			for (int i = 0; i < verticalRays; i++) {
				
				float lerpAmount = (float)i / (float)(verticalRays - 1);
				Vector2 origin = Vector2.Lerp (startPoint, endPoint, lerpAmount);
				//Ray ray = new Ray (origin, Vector2.down);
				
				hitInfos [i] = Physics2D.Raycast (origin, Vector2.down, distance , platformMask);
				
				if (hitInfos [i].fraction > 0 && hitInfos[i].collider.CompareTag("Platform")) {
					connected = true;
					if (lastFraction > 0) {
						float angle = Vector2.Angle (hitInfos [i].point - hitInfos [i - 1].point, Vector2.right);
						
						if (Mathf.Abs (angle /*- 90*/) < maxIncline) {
							break;
						}
					}
					amountConnected++;
					lastFraction = hitInfos [i].fraction;
				}
			}
			if (connected){
				//Debug.Log ("Gravity Raycast.fraction: " +hitInfos[indexUsed].fraction);
				grounded = true;
				falling = false;
			}
		}
	}

	public void Attack(){
		anim.SetTrigger("Attack");
	}

}
