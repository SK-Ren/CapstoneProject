using UnityEngine;
using System.Collections;

public class ActorInfo : MonoBehaviour {

	#region Attributes

	public float healthPoints;
	public float defenseMultiplier;
	public float damageMultiplier;
	
	#endregion

	#region Attribute TAGS

	class TAG {

		private ArrayList TAGS = new ArrayList<int>();

		enum TAG_List{
		INVULNERABLE,
		ALIVE,
		UNDEAD,
		INDESTRUCTIBLE,
		FIRE_IMMUNE,
		POISON_IMMUNE,
		CAN_FLY
		}

		public void addTAG(string TAG){
		

		}



	}

	// Use this for initialization
	void Start () {
	
	}

	/*ActorInfo (string[] Tags){
	}*/



	void Awake (){
		healthPoints = 10f;
		defenseMultiplier = 1f;
		damageMultiplier = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (healthPoints<=0)
			Destroy(gameObject);
	
	}

	public void takeDamage(){
		healthPoints -= 1f;

	}
}
