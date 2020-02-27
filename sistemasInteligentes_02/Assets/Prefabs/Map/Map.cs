using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour {
	public GameObject tilePrefab;
	MapBitMatrix matrix;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void setup(string mapFilePath){
		matrix = new MapBitMatrix (mapFilePath);
		drawMaze ();
	}

	public bool walkable_up (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("up of (" + (x) + ", " + (z) + ") is ("+ (x-1) + ", " + (z) + ") and is walkable = " + matrix.rows[x-1].columns[z]);
		return matrix.rows[x-1].columns[z];
	}

	public bool walkable_down (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("down of (" + (x) + ", " + (z) + ") is ("+ (x+1) + ", " + (z) + ") and is walkable = " + matrix.rows[x+1].columns[z]);
		return matrix.rows[x+1].columns[z];
	}

	public bool walkable_left (int x, int z){
		if (x < 1 || z < 1 || z > matrix.rows [0].columns.Length - 2 || x > matrix.rows.Length - 2)return false;
		//Debug.Log ("left of (" + (x) + ", " + (z) + ") is ("+ (x) + ", " + (z-1) + ") and is walkable = " +  matrix.rows[x].columns[z-1]);
		return matrix.rows[x].columns[z-1];
	}

	public bool walkable_right (int x, int z){
		if (x < 1 || z < 1 || z > matrix.rows [0].columns.Length - 2 || x > matrix.rows.Length - 2)return false;
		//Debug.Log ("right of (" + (x) + ", " + (z) + ") is ("+ (x) + ", " + (z+1) + ") and is walkable = " +  matrix.rows[x].columns[z+1]);
		return matrix.rows[x].columns[z+1];
	}

	public bool walkable_up_left (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("up left of (" + (x) + ", " + (z) + ") is ("+ (x-1) + ", " + (z-1) + ") and is walkable = " + matrix.rows[x-1].columns[z-1]);
		return matrix.rows[x-1].columns[z-1];
	}

	public bool walkable_up_right (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("up right of (" + (x) + ", " + (z) + ") is ("+ (x-1) + ", " + (z+1) + ") and is walkable = " + matrix.rows[x-1].columns[z+1]);
		return matrix.rows[x-1].columns[z+1];
	}

	public bool walkable_down_left (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("down left of (" + (x) + ", " + (z) + ") is ("+ (x+1) + ", " + (z-1) + ") and is walkable = " + matrix.rows[x+1].columns[z-1]);
		return matrix.rows[x+1].columns[z-1];
	}

	public bool walkable_down_right (int x, int z){
		if (x < 1 || x > matrix.rows.Length - 2 || z < 1 || z > matrix.rows [0].columns.Length - 2)return false;
		//Debug.Log ("down right of (" + (x) + ", " + (z) + ") is ("+ (x+1) + ", " + (z+1) + ") and is walkable = " + matrix.rows[x+1].columns[z+1]);
		return matrix.rows[x+1].columns[z+1];
	}

	void drawMaze(){
		//CACHE DIMENSIONS
		int matrixHeight = matrix.rows.Length;

		//CREATE EACH ROW
		for (int currentRow = 0; currentRow < matrixHeight; currentRow++) {
			MapBitRow row = matrix.rows [currentRow];
			GameObject rowObject = new GameObject ("row_" + currentRow);
			for (int currentColumn = 0; currentColumn < row.getLength(); currentColumn++) {
				GameObject tile = Instantiate (tilePrefab, new Vector3 (currentRow, 0, currentColumn), gameObject.transform.rotation) as GameObject;
				if (row.columns [currentColumn]) tile.GetComponent<Tile> ().setMaterial (tileType.Walkable);
				else tile.GetComponent<Tile> ().setMaterial (tileType.Blocked);
				tile.transform.SetParent (rowObject.transform);
			}
			rowObject.transform.SetParent (this.transform);
		}
	}

	[System.Serializable]
	public class MapBitRow{
		public bool [] columns;
		public int getLength(){
			return this.columns.Length;
		}
	}
	[System.Serializable]
	public class MapBitMatrix{
		public MapBitRow [] rows;

		public MapBitMatrix(string path){

			//==========================================================
			//CACHE DIMENSIONS
			//==========================================================
			int matrixHeight = 0;
			StreamReader reader = new StreamReader (path);
			while (reader.Peek () != -1) {
				reader.ReadLine ();
				matrixHeight++;
			}
			reader.Close ();

			//INIT
			this.rows = new MapBitRow [matrixHeight];
			for (int r = 0; r < matrixHeight; r ++) this.rows[r] = new MapBitRow();

			//==========================================================
			//CREATE EACH ROW
			//==========================================================
			reader = new StreamReader (path);
			int row = 0;
			while (reader.Peek () != -1) {
				string tempString = reader.ReadLine ();
				bool [] tempBitArray = new bool[tempString.Length];
				for (int column = 0; column < tempString.Length; column++){
					//COMPARE CHAR IN file.txt AND ASSIGN bool VALUE
					tempBitArray[column] = (
						(tempString[column] == '1') ||
						(tempString[column] == 'X') ||
						(tempString[column] == 'x') ||
						(tempString[column] == 'A') ||
						(tempString[column] == 'a')
					);
				}
				this.rows[row].columns = tempBitArray;
				row++;
			}
			reader.Close ();

			//==========================================================
			//FILL BLANKS TO PRODUCE RECTANGLE && ADD BORDERS
			//==========================================================
			normalize ();
		}

		int getMaxRowLength(){
			int temp = 0;
			for (int currentRow = 0; currentRow < this.rows.Length; currentRow++) {
				MapBitRow row = this.rows [currentRow];
				if (row.getLength () > temp) temp = row.getLength();
			}
			return temp;
		}
		void normalize(){
			int matrixLength = this.getMaxRowLength();
			int matrixHeight = this.rows.Length;
			MapBitRow [] newRows = new MapBitRow[matrixHeight + 2];
			for (int row = 0; row < matrixHeight + 2; row++) newRows [row] = new MapBitRow ();
			
			//==========================================================
			//CREATE EACH ROW
			//==========================================================
			for (int currentRow = 1; currentRow < matrixHeight+1; currentRow++) {
				bool [] row = new bool[matrixLength+2];
				//CREATE BORDERS
				row [0] = false;
				row [matrixLength + 1] = false;
				//CHECK EACH COLUMN OF THE ROW
				for (int currentColumn = 0; currentColumn < this.rows[currentRow-1].getLength (); currentColumn++) {
					row [currentColumn + 1] = this.rows [currentRow-1].columns [currentColumn];
				}
				//COMPLETE MISSING TILES
				for (int currentColumn = this.rows[currentRow-1].getLength (); currentColumn < matrixLength; currentColumn++) {
					row [currentColumn + 1] = false;
				}
				//ADD TO newRows
				newRows [currentRow].columns = row;
			}


			//==========================================================
			//CREATE BORDERS
			//==========================================================
			bool [] frontBorder = new bool[matrixLength+2];
			bool [] backBorder = new bool[matrixLength+2];
			for (int tile = 0; tile < matrixLength + 2; tile++) {
				frontBorder [tile] = false;
				backBorder [tile] = false;
			}
			newRows [0].columns = frontBorder;
			newRows [matrixHeight+1].columns = frontBorder;
			//Replace Matrix
			this.rows = newRows;
		}
	}
}
