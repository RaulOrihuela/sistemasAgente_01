using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SetupManager : MonoBehaviour {
	public static SetupManager instance;
	public GameObject mapPrefab;
	public GameObject agentPrefab;
	public string mapFile;

	void Awake(){
		if (instance == null) instance = this;
	}

	// Use this for initialization
	void Start () {
		//READ Agent's INITIAL POSITION
		int[] coordinates = agentCoordinates ("Assets/MapFiles/" + mapFile);

		//CREATE Map AND LOAD
		GameObject map = Instantiate (mapPrefab) as GameObject;
		map.transform.SetParent (this.transform);
		Map mapScript = map.GetComponent<Map> ();
		mapScript.setup ("Assets/MapFiles/" + mapFile);

		//CREATE Agent AND GIVE Map REFERENCE
		GameObject agent = Instantiate(agentPrefab, new Vector3 (coordinates[0],0,coordinates[1]), gameObject.transform.rotation) as GameObject; 
		agent.transform.SetParent (this.transform);
		agent.GetComponent<Agent> ().map = mapScript;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	int [] agentCoordinates (string path){
		int [] coordinates = new int[2];
		//DEFAULT VALUES
		coordinates [0] = 1;
		coordinates [1] = 1;

		int matrixHeight = 0;
		StreamReader reader = new StreamReader (path);
		while (reader.Peek () != -1) {
			string line = reader.ReadLine ();
			matrixHeight++;
			//SEARCH FOR 'A' or 'a'
			if (line.Contains ("A") || line.Contains ("a"))		 {
				//SEARCH FOR 'A' or 'a'
				int tempColumn = line.IndexOf ("A") + 1;
				if (tempColumn == -1) tempColumn = line.IndexOf ("a") + 1;
				//SET COORDINATE VALUES
				coordinates [0] = matrixHeight;
				coordinates [1] = tempColumn;
			}
		}
		reader.Close ();

		return coordinates;
	}
}
