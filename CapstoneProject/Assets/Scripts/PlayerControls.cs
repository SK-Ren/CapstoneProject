using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	
	protected ActorController2D actorControl; 

	// Use this for initialization
	void Start () 
	{
		actorControl = gameObject.GetComponent<ActorController2D> ();
	}
	
	// Update is called once per frame
	void Update () {

		
		if (Input.GetKeyDown(KeyCode.Space))
			actorControl.Jump();

		// Assigned Attack/Action Button
		if (Input.GetKeyDown (KeyCode.F))
			actorControl.Attack();

		// Actor class Horizontal Input
		actorControl.horizontalAxis = Input.GetAxis ("Horizontal");
	}

	void FixedUpdate()
	{

	}


}
