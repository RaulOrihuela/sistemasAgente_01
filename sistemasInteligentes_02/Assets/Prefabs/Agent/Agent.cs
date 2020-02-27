using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public Map map;
	[SerializeField]
	private int timeToLive;

	// Use this for initialization
	void Start () {
		this.transform.Rotate (0, -90, 0);
		timeToLive = 100;
		InvokeRepeating ("checkAdjacent",2,1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void destroy(){
		GameObject.Destroy (gameObject.transform.GetChild (1).gameObject);
		transform.DetachChildren ();
		GameObject.Destroy (this.gameObject);
	}

	void checkAdjacent(){
		timeToLive--;
		if (timeToLive < 0) destroy ();
		int myX = Mathf.RoundToInt(this.gameObject.transform.position.x), myZ = Mathf.RoundToInt(this.gameObject.transform.position.z);

		bool
		up = map.walkable_up (myX, myZ),
		up_right = map.walkable_up_right (myX, myZ),
		right = map.walkable_right (myX, myZ),
		down_right = map.walkable_down_right(myX, myZ),
		down = map.walkable_down (myX, myZ),
		down_left = map.walkable_down_left (myX, myZ),
		left = map.walkable_left (myX, myZ),
		up_left = map.walkable_up_left (myX, myZ);

		//==========================================================
		//AQUI VA LA PARTE DE PROGRAMAR EL COMPORTAMIENTO
		//==========================================================

		if (up) {
			this.transform.Translate (Vector3.forward);
		} else if (left) {
			this.transform.Translate (Vector3.left);
		} else if (down) {
			this.transform.Translate (Vector3.back);
		} else if (right) {
			this.transform.Translate (Vector3.right);
		}
	}
}
