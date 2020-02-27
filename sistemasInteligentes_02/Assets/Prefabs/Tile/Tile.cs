using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum tileType {Walkable, Blocked};
public class Tile : MonoBehaviour {
	private static Material whiteMaterial = null, blackMaterial = null;

	// Use this for initialization
	void Start () {
		//setMaterial (tileType.Walkable);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setMaterial(tileType type){
		if (type == tileType.Walkable) {
			if (whiteMaterial == null) {
				whiteMaterial = new Material (Shader.Find ("Standard"));
				whiteMaterial.color = new Color (1, 1, 1);
			}
			gameObject.GetComponent<Renderer> ().material = whiteMaterial;
		}
		if (type == tileType.Blocked) {
			if (blackMaterial == null) {
				blackMaterial = new Material (Shader.Find ("Standard"));
				blackMaterial.color = new Color (0, 0, 0);
			}
			gameObject.GetComponent<Renderer> ().material = blackMaterial;
		}
	}
}