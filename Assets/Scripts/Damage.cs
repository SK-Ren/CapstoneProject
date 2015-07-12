using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	
	// private ActorInfo _info;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void takeDamage(Collider2D col){
		Debug.Log ("takeDamage() reached");
		GameObject _colObj = col.gameObject;
		//Debug.Log ("Test");
		if (_colObj.layer == LayerMask.GetMask("Enemy"))
		    Debug.Log ("Hit Enemy"); // Knockback()

		if(_colObj.layer == LayerMask.GetMask("Weapon"))
		   	Debug.Log ("Hit Weapon"); // Knockback()

	}
}
