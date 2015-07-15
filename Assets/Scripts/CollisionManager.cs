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
		} else 
			Debug.Log ("Is Enemy: " + ( 1 <<col.gameObject.transform.parent.gameObject.layer == LayerMask.GetMask ("Enemy")));

		//if (col.gameObject.layer == LayerMask.GetMask ("Weapon"))
			//Debug.Log ("Collision: Weapon"); // Knockback()

	}
}