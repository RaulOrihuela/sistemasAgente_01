using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Minimax : MonoBehaviour {
	public Map map;
	public bool isMaximizer;
	public GameObject opponent;
	public int minimaxDepth;

	public AudioSource fatality;

	GameObject maxMesh;
	GameObject minMesh;

	Minimax_State initialState;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setInitialState(bool isMaxTurn){
		Vector2 myPos = new Vector2 (this.transform.position.x, this.transform.position.z),
		opponentPos = new Vector2 (opponent.transform.position.x, opponent.transform.position.z);

		if (this.isMaximizer)
			initialState = new Minimax_State (this.map, isMaxTurn,myPos,opponentPos);
		else 
			initialState = new Minimax_State (this.map, isMaxTurn,opponentPos,myPos);
	}

	public void showMesh(int child){
		maxMesh = gameObject.transform.Find ("Max").gameObject;
		minMesh = gameObject.transform.Find ("Min").gameObject;
		maxMesh.SetActive (child == 1);
		minMesh.SetActive (child == 2);
		gameObject.transform.Find ("Main Camera").gameObject.SetActive (child == 2);
	}

	public void move(){
		if (isMaximizer == initialState.isMaxTurn && initialState.posMax != initialState.posMin) {
			minimax (ref initialState, minimaxDepth, minimaxDepth, float.MinValue, float.MaxValue, isMaximizer);
			Vector3 movementDirection;
			if (this.isMaximizer) {
				movementDirection = new Vector3 (
					initialState.posMax.x - this.transform.position.x,
					0,
					initialState.posMax.y - this.transform.position.z
				);
				if (movementDirection.z == -1.0) maxMesh.transform.Rotate (0, 180 - maxMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.z == 1.0) maxMesh.transform.Rotate (0, - maxMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.x == -1.0) maxMesh.transform.Rotate (0, 270 - maxMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.x == 1.0) maxMesh.transform.Rotate (0, 90 - maxMesh.transform.rotation.eulerAngles.y, 0);
			} else {
				movementDirection = new Vector3 (
					initialState.posMin.x - this.transform.position.x,
					0,
					initialState.posMin.y - this.transform.position.z
				);
				if (movementDirection.z == -1.0) minMesh.transform.Rotate (0, 180 - minMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.z == 1.0) minMesh.transform.Rotate (0, - minMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.x == -1.0) minMesh.transform.Rotate (0, 270 - minMesh.transform.rotation.eulerAngles.y, 0);
				else if (movementDirection.x == 1.0) minMesh.transform.Rotate (0, 90 - minMesh.transform.rotation.eulerAngles.y, 0);
			}
			this.gameObject.transform.Translate (movementDirection);
			setInitialState (!initialState.isMaxTurn);
			opponent.GetComponent<Player_Minimax> ().setInitialState (initialState.isMaxTurn);
			if (initialState.posMax == initialState.posMin) {
				if (isMaximizer) showMesh (0);
				else opponent.GetComponent<Player_Minimax> ().showMesh (0);
				fatality.Play ();
				Debug.Log ("Game over");
			}
		}
	}


	float minimax(ref Minimax_State position, int initialDepth, int depth, float alpha, float beta, bool isMaximizing){
		//CHECK IF ROOT NODE OR END OF GAME
		if (position.posMax == position.posMin) return 0.0f;
		if (depth == 0) {
			Debug.Log ("LEAF NODE call at: max@(" + position.posMax.x + ", " +position.posMax.y + "), @min(" + position.posMin.x + ", " + position.posMin.y + ") value is: " + position.calculateHeuristic ());
			return position.calculateHeuristic ();
		}

		if (isMaximizing) {
			Debug.Log ("MAX NODE call at: max@(" + position.posMax.x + ", " +position.posMax.y + "), @min(" + position.posMin.x + ", " + position.posMin.y + ")");
			//Best found
			float maxScore = float.MinValue;
			Vector2 nextMove = new Vector2();
			//Get all children
			Minimax_State[] children = position.getChildren ().ToArray ();
			for (int tempPos = 0; tempPos < children.Length; tempPos++) {
				//Get score for each children
				float tempScore = minimax (ref children [tempPos], initialDepth, depth - 1, alpha, beta, false);
				//Update values
				if (tempScore > maxScore) {
					maxScore = tempScore;
					if (initialDepth == depth) nextMove = children [tempPos].posMax;
				}
				if (tempScore > alpha) alpha = tempScore;

				//Prune if necessary
				if (beta <= alpha) {
					Debug.Log ("alpa beta pruned");
					break;
				}
			}
			Debug.Log ("max node chose: " + maxScore);
			if (initialDepth == depth) {
				Debug.Log ("Next move to (" + nextMove.x + ", " + nextMove.y + ")");
				position.posMax = nextMove;
			}
			return maxScore;
		} else {
			Debug.Log ("MIN NODE call at: max@(" + position.posMax.x + ", " +position.posMax.y + "), @min(" + position.posMin.x + ", " + position.posMin.y + ")");
			//Best found
			float minScore = float.MaxValue;
			Vector2 nextMove = new Vector2();
			//Get all children
			Minimax_State[] children = position.getChildren ().ToArray ();
			for (int tempPos = 0; tempPos < children.Length; tempPos++) {
				//Get score for each children
				float tempScore = minimax (ref children [tempPos], initialDepth, depth - 1, alpha, beta, true);
				//Update values
				if (tempScore < minScore) {
					minScore = tempScore;
					if (initialDepth == depth) nextMove = children [tempPos].posMin;
				}
				if (tempScore < beta) beta = tempScore;

				//Prune if necessary
				if (beta <= alpha) {
					Debug.Log ("alpa beta pruned");
					break;
				}
			}
			Debug.Log ("min node chose: " + minScore);
			if (initialDepth == depth) {
				Debug.Log ("Next move to (" + nextMove.x + ", " + nextMove.y + ")");
				position.posMin = nextMove;
			}
			return minScore;
		}
	}

	[System.Serializable]
	public class Minimax_State{
		public Map map;
		public bool isMaxTurn;
		public Vector2 posMax;
		public Vector2 posMin;

		public Minimax_State(Map map, bool isMaxTurn, Vector2 posMax, Vector2 posMin){
			this.map = map;
			this.isMaxTurn = isMaxTurn;
			this.posMax = posMax;
			this.posMin = posMin;
		}

		public float calculateHeuristic(){
			if (isMaxTurn) return map.getHeuristics (posMax.x, posMax.y, posMin.x, posMin.y);
			else return  map.getHeuristics (posMin.x, posMin.y, posMax.x, posMax.y);
		}

		public List<Minimax_State> getChildren(){
			List<Minimax_State> result = new List<Minimax_State> ();

			if (this.isMaxTurn) {
				int posMax_x = Mathf.CeilToInt (this.posMax.x), posMax_y = Mathf.CeilToInt (this.posMax.y);

				if (this.map.walkable_up (posMax_x, posMax_y))
					result.Add (new Minimax_State (
						map, false,
						new Vector2 (this.posMax.x-1, this.posMax.y),
						this.posMin
					));
				if (this.map.walkable_down (posMax_x, posMax_y))
					result.Add (new Minimax_State (
						map, false,
						new Vector2 (this.posMax.x+1, this.posMax.y),
						this.posMin
					));
				if (this.map.walkable_left(posMax_x, posMax_y))
					result.Add (new Minimax_State (
						map, false,
						new Vector2 (this.posMax.x, this.posMax.y-1),
						this.posMin
					));
				if (this.map.walkable_right(posMax_x, posMax_y))
					result.Add (new Minimax_State (
						map, false,
						new Vector2 (this.posMax.x, this.posMax.y+1),
						this.posMin
					));
			} else {
				int posMin_x = Mathf.CeilToInt (this.posMin.x), posMin_y = Mathf.CeilToInt (this.posMin.y);

				if (this.map.walkable_up (posMin_x, posMin_y))
					result.Add (new Minimax_State (
						map, true,
						this.posMax,
						new Vector2 (this.posMin.x-1, this.posMin.y)
					));
				if (this.map.walkable_down (posMin_x, posMin_y))
					result.Add (new Minimax_State (
						map, true,
						this.posMax,
						new Vector2 (this.posMin.x+1, this.posMin.y)
					));
				if (this.map.walkable_left (posMin_x, posMin_y))
					result.Add (new Minimax_State (
						map, true,
						this.posMax,
						new Vector2 (this.posMin.x, this.posMin.y-1)
					));
				if (this.map.walkable_right (posMin_x, posMin_y))
					result.Add (new Minimax_State (
						map, true,
						this.posMax,
						new Vector2 (this.posMin.x, this.posMin.y+1)
					));
			}

			return result;
		}
	}
}
