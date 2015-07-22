using UnityEngine;
using System.Collections;
using Prime31;

public class CollisionManager : MonoBehaviour
{

	
	//private ActorInfo _info;
	//private PlayerControl _pControl;

	// Unsure if I want this accessible to the whole script
	//private GameObject _colObj;

	// Use this for initialization
	void Start ()
	{
		//_info = GetComponent<ActorInfo> ();
	 	//_pControl = GetComponent<PlayerControl> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Collision (Collider2D col)
	{
		//GameObject _colObj = col.gameObject;
		Debug.Log ("Collision() called");
		if ( 1 <<col.gameObject.transform.parent.gameObject.layer == LayerMask.GetMask ("Enemy")) {
			Debug.Log ("Collison: Enemy"); 
			SendMessageUpwards("Knockback");
			SendMessageUpwards("takeDamage");
			Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(),col); 
			//WaitForSeconds(1f);

		} else 
			Debug.Log ("Is Enemy: " + ( 1 <<col.gameObject.transform.parent.gameObject.layer == LayerMask.GetMask ("Enemy")));

		if ( 1 <<col.gameObject.transform.parent.gameObject.layer == LayerMask.GetMask ("Enemy")) {
			Debug.Log ("Collison: Enemy"); 
			SendMessageUpwards("Knockback");
			SendMessageUpwards("takeDamage");
			Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(),col); 
			//WaitForSeconds(1f);
			
		} else 
			Debug.Log ("Is Enemy: " + ( 1 <<col.gameObject.transform.parent.gameObject.layer == LayerMask.GetMask ("Enemy")));

	}
}