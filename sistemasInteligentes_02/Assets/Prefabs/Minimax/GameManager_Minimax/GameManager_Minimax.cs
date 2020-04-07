using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager_Minimax : MonoBehaviour {
	public static GameManager_Minimax instance;
	public GameObject mapPrefab;
	public GameObject minimaxPlayerPrefab;
	public string mapFile;
	[Range(2,6)]
	public int minimaxDepth;
	bool maxTurn = true;

	Player_Minimax maxPlayer_minimax, minPlayer_minimax;

	void Awake (){
		if (instance == null) instance = this;
	}

	// Use this for initialization
	void Start () {
		//READ Agent's INITIAL POSITION
		int[] coordinates = readCoordinates ("Assets/Prefabs/General/MapFiles/" + mapFile, "A", "a"), 
		goalCoordinates = readCoordinates("Assets/Prefabs/General/MapFiles/" + mapFile, "X", "x");

		//CREATE Map AND LOAD
		GameObject map = Instantiate (mapPrefab) as GameObject;
		map.transform.SetParent (this.transform);
		Map mapScript = map.GetComponent<Map> ();
		mapScript.setup ("Assets/Prefabs/General/MapFiles/" + mapFile);
		mapScript.bakeHeuristics (heuristicBake.Manhattan);

		GameObject maxPlayer = Instantiate(minimaxPlayerPrefab, new Vector3 (coordinates[0],0,coordinates[1]), gameObject.transform.rotation) as GameObject; 
		maxPlayer.transform.SetParent (this.transform);
		maxPlayer_minimax = maxPlayer.GetComponent<Player_Minimax> ();
		maxPlayer_minimax.map = mapScript;
		maxPlayer_minimax.isMaximizer = true;
		maxPlayer_minimax.minimaxDepth = this.minimaxDepth;

		GameObject minPlayer = Instantiate(minimaxPlayerPrefab, new Vector3 (goalCoordinates[0],0,goalCoordinates[1]), gameObject.transform.rotation) as GameObject; 
		minPlayer.transform.SetParent (this.transform);
		minPlayer_minimax = minPlayer.GetComponent<Player_Minimax> ();
		minPlayer_minimax.map = mapScript;
		minPlayer_minimax.isMaximizer = false;
		minPlayer_minimax.minimaxDepth = this.minimaxDepth;

		maxPlayer_minimax.opponent = minPlayer;
		minPlayer_minimax.opponent = maxPlayer;

		maxPlayer_minimax.setInitialState (maxTurn);
		minPlayer_minimax.setInitialState (maxTurn);

		maxPlayer_minimax.showMesh (1);
		minPlayer_minimax.showMesh (2);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			if (maxTurn) maxPlayer_minimax.move ();
			else minPlayer_minimax.move ();
			maxTurn = !maxTurn;
		}
	}

	int [] readCoordinates (string path, string patternA, string patternB){
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
			if (line.Contains (patternA) || line.Contains (patternB))		 {
				//SEARCH FOR 'A' or 'a'
				int tempColumn = line.IndexOf (patternA) + 1;
				if (tempColumn == -1) tempColumn = line.IndexOf (patternB) + 1;
				//SET COORDINATE VALUES
				coordinates [0] = matrixHeight;
				coordinates [1] = tempColumn;
			}
		}
		reader.Close ();

		return coordinates;
	}
}
