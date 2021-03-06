﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cerebro {

	public static class GroupMapping {
		public const string Group1 = "1";
		public const string Group2 = "2";
		public const string Group3 = "3";
		public const string Group4 = "4";
		public static Color Color1 = new Color(0.99f, 0.64f, 0f);
		public static Color Color2 = new Color(0.05f, 0.9f, 0.9f);
		public static Color Color3 = new Color(0.98f, 0.6f, 0.98f);
		public static Color Color4 = new Color(0.39f, 0.62f, 0.92f);
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

		public void DisableAllComponents(Unit unit)
		{
			SpriteRenderer[] childs = unit.transform.GetComponentsInChildren<SpriteRenderer> ();
			for (int i = 0; childs != null && i < childs.Length; i++) {
				childs [i].gameObject.SetActive (false);
			}
			unit.gameObject.SetActive (true);
			unit.transform.FindChild ("Marker").gameObject.SetActive (true);
//			childs = unit.transform.FindChild ("baba_head_without_eyes").GetComponentsInChildren<SpriteRenderer> ();
//			for (int i = 0; childs != null && i < childs.Length; i++) {
//				childs [i].gameObject.SetActive (false);
//			}
			unit.transform.FindChild ("baba_head_without_eyes").gameObject.SetActive (true);
//			childs = unit.transform.FindChild ("baba_hats").GetComponentsInChildren<SpriteRenderer> ();
//			for (int i = 0; childs != null && i < childs.Length; i++) {
//				childs [i].gameObject.SetActive (false);
//			}
			unit.transform.FindChild ("baba_hats").gameObject.SetActive (true);
//			childs = unit.transform.FindChild ("baba_badges").GetComponentsInChildren<SpriteRenderer> ();
//			for (int i = 0; childs != null && i < childs.Length; i++) {
//				childs [i].gameObject.SetActive (false);
//			}
			unit.transform.FindChild ("baba_badges").gameObject.SetActive (true);
//			childs = unit.transform.FindChild ("baba_goggles").GetComponentsInChildren<SpriteRenderer> ();
//			for (int i = 0; childs != null && i < childs.Length; i++) {
//				childs [i].gameObject.SetActive (false);
//			}
			unit.transform.FindChild ("baba_goggles").gameObject.SetActive (true);
		}

		public void ChooseUnit(string groupID, Unit unit, int hairID = 1, int faceID = 1, int bodyID = 1, int hatID = 0, int gogglesID = 0, int badgesID = 0)
		{
			DisableAllComponents (unit);
//			Debug.Log ("here "+unit.Cell.cellIndex+" hat "+hatID+" badge "+badgesID);
			hairID = Mathf.Clamp (hairID, 1, 8);
			faceID = Mathf.Clamp (faceID, 1, 8);
			bodyID = Mathf.Clamp (bodyID, 1, 8);

			var hairString = "baba_hair_1";
			var hairString2 = "";
			var bodyString = "baba_body_1";
			var faceString = "baba_head_1";

			if (hairID == 1) {
				hairString = "baba_hair_1";
			} else if (hairID == 2) {
				hairString = "baba_hair_2";
			} else if (hairID == 3) {
				hairString = "baba_hair_3";
			} else if (hairID == 4) {
				hairString = "baba_hair_4";
			} else if (hairID == 5) {
				hairString = "baba_hair_5_front";
				hairString2 = "baba_hair_5_back";
			} else if (hairID == 6) {
				hairString = "baba_hair_6_front";
			} else if (hairID == 7) {
				hairString = "baba_hair_7_front";
				hairString2 = "baba_hair_7_back";
			} else if (hairID == 8) {
				hairString = "baba_hair_8_front";
				hairString2 = "baba_hair_8_back";
			}

			bodyString = "baba_body_" + bodyID.ToString();
			faceString = "baba_head_" + faceID.ToString();

			var body = unit.transform.Find (bodyString);

			bool IsBoy = faceID > 4 ? false : true;

			if (gogglesID > 0) {
				var face = unit.transform.FindChild("baba_head_without_eyes").transform.Find (faceString);
				face.gameObject.SetActive (true);
				string goggleString = "";
				if (IsBoy) {
					goggleString = "boy_goggles_" + gogglesID;
				} else {
					goggleString = "girl_goggles_" + gogglesID;
				}
				var goggle = unit.transform.FindChild("baba_goggles").transform.Find (goggleString);
				goggle.gameObject.SetActive (true);
			} else {
				var face = unit.transform.Find (faceString);
				face.gameObject.SetActive (true);
			}

			body.gameObject.SetActive (true);

			Transform hair = null;
			SpriteRenderer hair2Rndr = null;
			SpriteRenderer hairRndr = null;
			if (hatID > 0) {
				string hatString = "";
				if (IsBoy) {
					hatString = "boy_hats_" + hatID;
				} else {
					hatString = "girl_hats_" + hatID;
				}
				var hat = unit.transform.FindChild("baba_hats").transform.Find (hatString);
				hat.gameObject.SetActive (true);
			} else {
				hair = unit.transform.Find (hairString);
				Transform hair2 = null;
				if (hairString != "") {
					hair2 = unit.transform.Find (hairString2);
				}
				hair.gameObject.SetActive (true);
				if (hair2) {
					hair2.gameObject.SetActive (true);
					hair2Rndr = hair2.GetComponent<SpriteRenderer> ();
				}
				hairRndr =  hair.GetComponent<SpriteRenderer> ();
			}
			var bodyRndr = body.GetComponent<SpriteRenderer> ();
			if (badgesID > 0) {
				string badgeString = "";
				if (IsBoy) {
					badgeString = "boy_badges_" + badgesID;
				} else {
					badgeString = "girl_badges_" + badgesID;
				}
				var badge = unit.transform.FindChild("baba_badges").transform.Find (badgeString);
				badge.gameObject.SetActive (true);
			}

			var teamColor = new Color (0, 0, 0);
			if (groupID == GroupMapping.Group1) {
				teamColor = GroupMapping.Color1;
			} else if (groupID == GroupMapping.Group2) {
				teamColor = GroupMapping.Color2;
			} else if (groupID == GroupMapping.Group3) {
				teamColor = GroupMapping.Color3;
			} else if (groupID == GroupMapping.Group4) {
				teamColor = GroupMapping.Color4;
			}
			if(hair2Rndr)
			hair2Rndr.color = teamColor;
			bodyRndr.color = teamColor;
			if(hairRndr)
			hairRndr.color = teamColor;
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
					GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
					unit = unitGameObj.GetComponent<Unit>();
					unit.PlayerNumber = 0;
				} else if (cell.groupID == GroupMapping.Group2) {
					GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
					unit = unitGameObj.GetComponent<Unit>();
					unit.PlayerNumber = 1;
				} else if (cell.groupID == GroupMapping.Group3) {
					GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
					unit = unitGameObj.GetComponent<Unit>();
					unit.PlayerNumber = 2;
				} else if (cell.groupID == GroupMapping.Group4) {
					GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
					unit = unitGameObj.GetComponent<Unit>();
					unit.PlayerNumber = 3;
				}

				if (unit != null) {
					cell.IsTaken = true;
					unit.gameObject.transform.localScale = new Vector3 (0.15f, 0.15f, 1);  // SET SCALE HERE
					unit.Cell = cell;
					unit.transform.position = cell.transform.position;
					unit.Initialize ();
					Debug.Log ("spawn hair "+cell.BabaHairId+" face "+cell.BabaFaceId+" body "+cell.BabaBodyId);
					ChooseUnit (cell.groupID, unit, cell.BabaHairId, cell.BabaFaceId, cell.BabaBodyId, cell.BabaHatId, cell.BabaGogglesId, cell.BabaBadgeId);

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

		public Unit AddNewUnit(Cell cell, string groupID, int HairId = 1, int FaceId = 1, int BodyId = 1, int HatId = 0, int GogglesId = 0, int BadgeId = 0) {
			Unit unit = null;

			if (groupID == GroupMapping.Group1) {
				GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
				unit = unitGameObj.GetComponent<Unit>();
				unit.PlayerNumber = 0;
			} else if (groupID == GroupMapping.Group2) {
				GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
				unit = unitGameObj.GetComponent<Unit>();
				unit.PlayerNumber = 1;
			} else if (groupID == GroupMapping.Group3) {
				GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
				unit = unitGameObj.GetComponent<Unit>();
				unit.PlayerNumber = 2;
			} else if (groupID == GroupMapping.Group4) {
				GameObject unitGameObj = Cerebro.PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Group1Player, UnitsParent.transform);
				unit = unitGameObj.GetComponent<Unit>();
				unit.PlayerNumber = 3;
			}


			if (unit != null) {
				cell.IsTaken = true;
				unit.gameObject.transform.localScale = new Vector3 (0.15f, 0.15f, 1);  // SET SCALE HERE
				unit.Cell = cell;
				unit.transform.position = cell.transform.position;
				ChooseUnit (groupID, unit, HairId, FaceId, BodyId, HatId, GogglesId, BadgeId);
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
