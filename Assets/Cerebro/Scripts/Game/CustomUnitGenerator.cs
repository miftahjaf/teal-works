using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cerebro {

	public static class GroupMapping {
		public const string Group1 = "1";
		public const string Group2 = "2";
		public const string Group3 = "3";
		public const string Group4 = "4";
	}

	public class CustomUnitGenerator : MonoBehaviour, IUnitGenerator
	{
		string currentGameID;

	    public Transform UnitsParent;
	    public Transform CellsParent;
		public string currentStudentID;
		public string currentGroupID;
		public int currentGroupNumber;

		private GameObject mCurrentGameObject;
		public GameObject mCurrentUnit;

		void Start() {
			currentGameID = transform.parent.GetComponent<GOTGame> ().CurrentGameID;

			for (var i = 0; i < CellsParent.transform.childCount; i++) {
				float xVal = i % 8;
				float yVal = Mathf.Floor (i / 8f);
				CellsParent.transform.GetChild (i).gameObject.GetComponent<Cell> ().OffsetCoord = new Vector2 (xVal, yVal);
			}

			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			LaunchList.instance.GetGame (currentGameID, studentID, GameDataLoaded);
		}

		void GameDataLoaded (int dummy) {

			currentStudentID = LaunchList.instance.mCurrentGame.StudentID;
			currentGroupID = LaunchList.instance.mCurrentGame.GroupID;
			if (currentGroupID == GroupMapping.Group4) {
				currentGroupNumber = 3;
			} else if (currentGroupID == GroupMapping.Group3) {
				currentGroupNumber = 2;
			} else if (currentGroupID == GroupMapping.Group2) {
				currentGroupNumber = 1;
			} else if (currentGroupID == GroupMapping.Group1) {
				currentGroupNumber = 0;
			}
			if (this != null && this.gameObject != null) {
				GetComponent<CellGrid> ().GameDataLoaded ();
			}
		}
	    /// <summary>
	    /// Returns units that are already children of UnitsParent object.
	    /// </summary>
	    public List<Unit> SpawnUnits(List<Cell> cells)
	    {
	        List<Unit> ret = new List<Unit>();
			for (int i = 0; i < cells.Count; i++) {
				var cell = cells [i];
				Unit unit = null;
				if (cell.groupID == GroupMapping.Group1) {
					unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform).GetComponent<Unit>();
				} else if (cell.groupID == GroupMapping.Group2) {
					unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group2Player, UnitsParent.transform).GetComponent<Unit>();
				} else if (cell.groupID == GroupMapping.Group3) {
					unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group3Player, UnitsParent.transform).GetComponent<Unit>();
				} else if (cell.groupID == GroupMapping.Group4) {
					unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group4Player, UnitsParent.transform).GetComponent<Unit>();
				}
				if (unit != null) {
					cell.IsTaken = true;
					unit.gameObject.transform.localScale = new Vector3 (0.15f, 0.15f, 1);  // SET SCALE HERE
					unit.Cell = cell;
					unit.transform.position = cell.transform.position;
					unit.Initialize ();
					ret.Add (unit);
					mCurrentGameObject = unit.gameObject;
				}
			}
			return ret;
	    }

		public GameObject getCurrentGameObject() {
			return mCurrentGameObject;
		}

		public void RemoveAllUnits() {
			while (UnitsParent.transform.childCount != 0) {
				GameObject deleteUnit = UnitsParent.transform.GetChild (0).gameObject;
				Unit unit = deleteUnit.GetComponent<Unit> ();
				unit.Cell.IsTaken = false;
				unit.MarkAsDestroyed ();
				DestroyImmediate (deleteUnit);
			}
		}
		public List<Unit> GetAllUnits() {
			List<Unit> ret = new List<Unit> ();
			for (int i = 0; i < UnitsParent.childCount; i++) {
				var unit = UnitsParent.GetChild (i).GetComponent<Unit> ();
				ret.Add (unit);
			}
			return ret;
		}

		public Unit AddNewUnit(Cell cell, string groupID) {
			Unit unit = null;
			if (groupID == GroupMapping.Group1) {
				unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform).GetComponent<Unit>();
			} else if (groupID == GroupMapping.Group2) {
				unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group2Player, UnitsParent.transform).GetComponent<Unit>();
			} else if (groupID == GroupMapping.Group3) {
				unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group3Player, UnitsParent.transform).GetComponent<Unit>();
			} else if (groupID == GroupMapping.Group4) {
				unit = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group4Player, UnitsParent.transform).GetComponent<Unit>();
			}
			if (unit != null) {
				cell.IsTaken = true;
				unit.gameObject.transform.localScale = new Vector3 (0.15f, 0.15f, 1);  // SET SCALE HERE
				unit.Cell = cell;
				unit.transform.position = cell.transform.position;
				unit.Initialize ();
			}
			return unit;
		}

		public Unit spawnNewUnit(List<Cell> cells, GameObject newUnit, bool informServerBool = true, bool shouldWait = true) {
			var unit = newUnit.GetComponent<Unit>();
			if (unit != null) {
				var cell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
				if (!cell.IsTaken) {
					cell.IsTaken = true;
					unit.Cell = cell;
					unit.transform.position = cell.transform.position;
					unit.Initialize ();
					if (shouldWait) {
						unit.MarkAsWaiting ();
					}
					cell.groupID = currentGroupID;
					cell.studentID = currentStudentID;
					cell.MovementCost += 5;

					mCurrentUnit = unit.gameObject;
					if(informServerBool) {
						HTTPRequestHelper.instance.ValidateMove (cell.cellIndex.ToString (), cell.MovementCost.ToString (), cell.studentID, LaunchList.instance.mCurrentGame.GroupIDDB, currentGameID);
					}
					return unit;
				} else {
					Destroy(unit.gameObject);
				}
			}
			return null;
		}

		public void DeleteUnit(GameObject deleteUnit) {
			Unit unit = deleteUnit.GetComponent<Unit> ();
			unit.Cell.IsTaken = false;
			unit.MarkAsDestroyed ();
		}

	    public void SnapToGrid()
	    {
	        List<Transform> cells = new List<Transform>();

	        foreach(Transform cell in CellsParent)
	        {
	            cells.Add(cell);
	        }

	        foreach(Transform unit in UnitsParent)
	        {
	            var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
	            if (!closestCell.GetComponent<Cell>().IsTaken)
	            {
	                Vector3 offset = new Vector3(0,0, closestCell.GetComponent<Cell>().GetCellDimensions().z);
	                unit.position = closestCell.transform.position - offset;
	            }//Unit gets snapped to the nearest cell
	        }
	    }
	}

}
