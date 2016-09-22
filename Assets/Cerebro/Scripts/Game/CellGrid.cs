using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

/// <summary>
/// CellGrid class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions.
/// It reacts to user interacting with units or cells, and raises events related to game progress.
/// </summary>
///
///
/// To Replay feature: UnComment StartCoroutine (ReproduceGame ());, Comment 2 lines in Start() - LaunchList.instance.GetWorld (); SetWorld (true); , Update function, LaunchList update function - ProcessMoves();
namespace Cerebro
{

	public static class GameStatuses
	{
		public const string WaitingForServer = "ATTEMPTING ATTACK";
		public const string isInvincible = "CAN'T CLICK THAT BASE";
		public const string noCoins = "MORE COINS REQUIRED";
		public const string Success = "SUCCESS!";
		public const string Failure = "YIKES!";
		public const string Wait = "HOLD YOUR HORSES!";
	}

	public class CellGrid : MonoBehaviour
	{
		public event EventHandler GameStarted;
		public event EventHandler GameEnded;
		public event EventHandler TurnEnded;

		public GameObject callOut;

		public UnityEngine.UI.Text CallOutValueText;
		public UnityEngine.UI.Text coinsText;
		public UnityEngine.UI.Text timerText;
		public UnityEngine.UI.Text statusText;
		public Text countDisplay;

		private int decrementScore = 0;
		private int decrementBy = 0;
		private int currentScore = 0;

		private float updateCoinsCntr = 0f;

		private float updateCount = 0f;

		string currentGameID;

		private Coroutine AnimateTextCoroutine;

		private CellGridState _cellGridState;
		//The grid delegates some of its behaviours to cellGridState object.
		public CellGridState CellGridState {
			private get {
				return _cellGridState;
			}
			set {
				if (_cellGridState != null)
					_cellGridState.OnStateExit ();
				_cellGridState = value;
				_cellGridState.OnStateEnter ();
			}
		}

		public int NumberOfPlayers { get; private set; }

		public int PlayerCoins;

		public Player CurrentPlayer {
			get { return Players.Find (p => p.PlayerNumber.Equals (CurrentPlayerNumber)); }
		}

		public int CurrentPlayerNumber { get; private set; }

		public Transform PlayersParent;

		public List<Player> Players { get; private set; }

		public List<Cell> Cells { get; private set; }

		public List<Unit> Units { get; private set; }

		private bool callOutActive = false;

		private Cell mCurrentCellToAcquire;
		private Unit mCurrentFromUnit;
		private Unit mCurrentToUnit;

		private bool waitingForGameData = true;

		void Start ()
		{
			currentGameID = transform.parent.GetComponent<GOTGame> ().CurrentGameID;
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.GOT);

			LaunchList.instance.WorldChanged += GameWorldChanged;
			HTTPRequestHelper.instance.MoveValidated += MoveValidated;

			Players = new List<Player> ();
			for (int i = 0; i < PlayersParent.childCount; i++) {
				var player = PlayersParent.GetChild (i).GetComponent<Player> ();
				if (player != null) {
					Players.Add (player);
				} else
					CerebroHelper.DebugLog ("Invalid object in Players Parent game object");
			}
			NumberOfPlayers = Players.Count;

			PlayerCoins = LaunchList.instance.mCurrentStudent.Coins;
			coinsText.text = "Coins: " + PlayerCoins;
			timerText.text = "Time Left: calculating...";
			currentScore = PlayerCoins;

			SetStatusText (GameStatuses.Wait, false);

			waitingForGameData = true;

			//hide units during start
			GetComponent<CustomUnitGenerator> ().UnitsParent.gameObject.SetActive (false);
//			StartCoroutine (ReproduceGame ());
		}

		public void GameDataLoaded() {
			waitingForGameData = false;
			CurrentPlayerNumber = GetComponent<CustomUnitGenerator> ().currentGroupNumber;
			GetComponent<CustomUnitGenerator> ().UnitsParent.gameObject.SetActive (true);
			SetWorld (true);
		}

		IEnumerator ReproduceGame ()
		{
			for (var i = 0; i < 96; i++) {
				var newRecord = new World ();
				newRecord.CellID = i.ToString ();
				newRecord.Cost = "30";
				if (i == 0) {
					newRecord.GroupID = GroupMapping.Group1;
					newRecord.StudentID = "314";
				} else if (i == 88) {
					newRecord.GroupID = GroupMapping.Group2;
					newRecord.StudentID = "315";
				} else if (i == 95) {
					newRecord.GroupID = GroupMapping.Group3;
					newRecord.StudentID = "316";
				} else if (i == 7) {
					newRecord.GroupID = GroupMapping.Group4;
					newRecord.StudentID = "317";
				} else {
					newRecord.GroupID = "-1";
					newRecord.StudentID = "-1";
				}
				Cerebro.LaunchList.instance.mWorld.Add (newRecord);
			}

			string fileName = Application.persistentDataPath + "/Moves-Game1.csv";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						var studentID = line.Split ("," [0]) [0].Replace ("\"", "");
						var cellID = line.Split ("," [0]) [2].Replace ("\"", "");
						var groupID = line.Split ("," [0]) [3].Replace ("\"", "");
						var cellCost = line.Split ("," [0]) [4].Replace ("\"", "");
						var moveValid = line.Split ("," [0]) [5].Replace ("\"", "");
						if (moveValid == "1") {
							var cellIndex = LaunchList.instance.FindCell (cellID);
							Cerebro.LaunchList.instance.mWorld [cellIndex].Cost = cellCost;
							Cerebro.LaunchList.instance.mWorld [cellIndex].StudentID = studentID;
							Cerebro.LaunchList.instance.mWorld [cellIndex].GroupID = groupID;
							GameWorldChanged (null, null);
							yield return new WaitForSeconds (0.00001f);
						}
					}
				}  
				sr.Close ();
			}
		}

		public void SetStatusText (string str, bool hideBool = true)
		{
			if (AnimateTextCoroutine != null) {
				StopCoroutine (AnimateTextCoroutine);
				AnimateTextCoroutine = null;
			}
			statusText.gameObject.transform.localPosition = new Vector3 (statusText.gameObject.transform.localPosition.x, statusText.gameObject.transform.localPosition.y - 200, statusText.gameObject.transform.localPosition.z);
			statusText.text = str;
			if (hideBool) {
				AnimateTextCoroutine = StartCoroutine (AnimateText ());
			}
		}

		IEnumerator AnimateText ()
		{
			print ("AnimateText Start");
			Go.to (statusText.gameObject.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (0, 15, 0), false));
			yield return new WaitForSeconds (2f);
			print ("AnimateText End");
			Go.to (statusText.gameObject.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (0, -200, 0), true));
		}

		private void SetWorld (bool firstTime = false)
		{
			if (this != null) {
				
				SetCells ();

				foreach (var cell in Cells) {
					cell.CellClicked += OnCellClicked;
					cell.CellHighlighted += OnCellHighlighted;
					cell.CellDehighlighted += OnCellDehighlighted;
				}

				var unitGenerator = GetComponent<IUnitGenerator> ();
				if (unitGenerator != null) {
					Units = unitGenerator.SpawnUnits (Cells);
					foreach (var unit in Units) {
						unit.UnitClicked += OnUnitClicked;
						unit.UnitDestroyed += OnUnitDestroyed;
					}
				} else
					CerebroHelper.DebugLog ("No IUnitGenerator script attached to cell grid");

				CurrentPlayerNumber = GetComponent<CustomUnitGenerator> ().currentGroupNumber;
				StartGame ();

				if (LaunchList.instance.mCurrentMove != null) {
					CurrentPlayerNumber = -1;
					var customUnitGenerator = GetComponent<CustomUnitGenerator> ();
					GameObject newUnit = Instantiate (customUnitGenerator.getCurrentGameObject ());
					newUnit.transform.SetParent (customUnitGenerator.getCurrentGameObject ().transform.parent);
					newUnit.transform.localScale = customUnitGenerator.getCurrentGameObject ().transform.localScale;

					GameObject cell = GetCellWithID (Int32.Parse (LaunchList.instance.mCurrentMove.CellID)).gameObject;
					newUnit.transform.position = cell.transform.position;
					CellGrid cg = GameObject.Find ("CellGrid").GetComponent<CellGrid> ();

					if (customUnitGenerator != null) {
						var unit = customUnitGenerator.spawnNewUnit (Cells, newUnit, false);
						unit.UnitClicked += OnUnitClicked;
						unit.UnitDestroyed += OnUnitDestroyed;
					}
					Units = customUnitGenerator.GetAllUnits ();
					CellGridState = new CellGridStateUnitsSelected (this, Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)));
				} else { 
					if (firstTime) {
						CurrentPlayerNumber = -1;
						CellGridState = new CellGridStateUnitsSelected (this, Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)));
						HTTPRequestHelper.instance.GetGOTWorld (currentGameID);
					} else {
						SetStatusText ("");
					}
				}
			}
		}

		private void SetCells ()
		{
			if (Cells != null) {
				Cells.Clear ();
			} else {
				Cells = new List<Cell> ();
			}

			for (int i = 0; i < Cerebro.LaunchList.instance.mWorld.Count; i++) {
				var currentCell = Cerebro.LaunchList.instance.mWorld [i];
				Debug.Log ("cell id "+currentCell.CellID);
				var index = int.Parse (currentCell.CellID);
				var cell = transform.GetChild (index).gameObject.GetComponent<Cell> ();
				cell.cellIndex = index;
				if (cell.cellIndex == 0 || cell.cellIndex == 7 || cell.cellIndex == 88 || cell.cellIndex == 95) {
					cell.isInvincible = true;
				} else {
					cell.isInvincible = false;
				}
				cell.BabaHairId = Cerebro.LaunchList.instance.mWorld [i].BabaHairId;
				cell.BabaFaceId = Cerebro.LaunchList.instance.mWorld [i].BabaFaceId;
				cell.BabaBodyId = Cerebro.LaunchList.instance.mWorld [i].BabaBodyId;

				cell.OffsetCoord = new Vector2 (index % 8, Mathf.Floor (index / 8f));
				cell.studentID = currentCell.StudentID;
				cell.groupID = currentCell.GroupID;
				//CerebroHelper.DebugLog ("CELL OFFSET AT CREATE " + cell.OffsetCoord.x + " " + cell.OffsetCoord.y);
				cell.MovementCost = int.Parse (currentCell.Cost);

				if (cell != null) {
					Cells.Add (cell);
				} else
					CerebroHelper.DebugLog ("Invalid object in cells paretn game object");
			}
		}

		private Cell GetCellWithID (int index)
		{
			for (var i = 0; i < Cells.Count; i++) {
				if (Cells [i].cellIndex == index) {
					return Cells [i];
				}
			}
			return null;
		}

		private void IncrementWorld() {
			if (this == null || this.gameObject == null) {
				return;
			}
			var customGenerator = GetComponent<CustomUnitGenerator> ();
			var units = customGenerator.GetAllUnits ();
			if (!LaunchList.instance.mWaitingforMovesandWorldServer && this != null) {
				for (int i = 0; i < Cerebro.LaunchList.instance.mWorld.Count; i++) {
					var cellData = Cerebro.LaunchList.instance.mWorld [i];
					var index = int.Parse (cellData.CellID);
					var cell = transform.GetChild (index).gameObject.GetComponent<Cell> ();
					var createCell = true;
					foreach (var unit in units) {
						if (index == unit.Cell.cellIndex) {
							createCell = false;
							if (unit.Cell.groupID != cellData.GroupID) {
								DeleteUnit (unit.gameObject);
								createCell = true;
							} else if (unit.isWaiting) {
								unit.MarkAsReady ();
							}
						}
					}
					if (createCell && cellData.GroupID != "-1") {
						var unit = customGenerator.AddNewUnit (cell, cellData.GroupID, cellData.BabaHairId, cellData.BabaFaceId, cellData.BabaBodyId);
						unit.UnitClicked += OnUnitClicked;
						unit.UnitDestroyed += OnUnitDestroyed;
					}
				}

				SetCells ();
				Units = customGenerator.GetAllUnits ();
				CurrentPlayerNumber = customGenerator.currentGroupNumber;
				StartGame ();
			}
		}

		private void GameWorldChanged (object sender, System.EventArgs e)
		{
			CerebroHelper.DebugLog ("GameWorldChanged");
			if (Cells == null || Cells.Count == 0) {
				CerebroHelper.DebugLog ("SetWorld");
				SetWorld ();
			} else {
				CerebroHelper.DebugLog ("IncrementWorld");
				IncrementWorld ();
			}
		}

		void MoveValidated (object sender, EventArgs e)
		{
			if (this == null) {
				return;
			}
			var customGenerator = GetComponent<CustomUnitGenerator> ();
			if ((e as MoveValidateArgs).Valid == true) {
				SetStatusText (GameStatuses.Success);
				UseCoinsOf ((e as MoveValidateArgs).Coins);
			} else {
				SetStatusText (GameStatuses.Failure);
			}

			LaunchList.instance.mWaitingforMovesandWorldServer = false;

			LaunchList.instance.mCurrentMove = null;
			CurrentPlayerNumber = customGenerator.currentGroupNumber;
			HTTPRequestHelper.instance.GetGOTWorld (currentGameID);
		}
			
		private void RemoveUnitsFromWorld ()
		{
			if (this != null) {
				var customGenerator = GetComponent<CustomUnitGenerator> ();
				customGenerator.RemoveAllUnits ();
			}
		}

		public int GetCoins ()
		{
			return PlayerCoins;
		}

		public void UseCoinsOf (int Coins)
		{
			decrementScore = Coins;
			decrementBy = Mathf.FloorToInt (Coins / 10f);
			LaunchList.instance.SetCoins(-Coins);
			PlayerCoins = (LaunchList.instance.mCurrentStudent.Coins);
//			coinsText.text = "Coins: " + PlayerCoins;
		}

		public void AddUnit (GameObject newUnit)
		{
			var unitGenerator = GetComponent<CustomUnitGenerator> ();
			if (unitGenerator != null) {
				var unit = unitGenerator.spawnNewUnit (Cells, newUnit);
				unit.UnitClicked += OnUnitClicked;
				unit.UnitDestroyed += OnUnitDestroyed;
			}
			Units = unitGenerator.GetAllUnits ();
			CellGridState = new CellGridStateUnitsSelected (this, Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)));
		}

		public void DeleteUnit (GameObject deletedUnit)
		{
			var unitGenerator = GetComponent<CustomUnitGenerator> ();
			unitGenerator.DeleteUnit (deletedUnit);
			DestroyImmediate (deletedUnit);
			Units = unitGenerator.GetAllUnits ();
		}

		public void updateCellCost (Cell cell, int newCost)
		{
//			var customGenerator = GetComponent<CustomUnitGenerator>();
//			LaunchList.instance.MakeMove (cell.cellIndex.ToString(), customGenerator.currentStudentID,customGenerator.currentGroupID, newCost.ToString ());
		}

		private void OnCellDehighlighted (object sender, EventArgs e)
		{
			CellGridState.OnCellDeselected (sender as Cell);
		}

		private void OnCellHighlighted (object sender, EventArgs e)
		{
			CellGridState.OnCellSelected (sender as Cell);
		}

		private void OnCellClicked (object sender, EventArgs e)
		{
			CellGridState.OnCellClicked (sender as Cell);
		}

		public void OnUnitClicked (object sender, EventArgs e)
		{
			CellGridState.OnUnitClicked (sender as Unit);
		}

		public void OnUnitDestroyed (object sender, AttackEventArgs e)
		{
			Units.Remove (sender as Unit);
			var totalPlayersAlive = Units.Select (u => u.PlayerNumber).Distinct ().ToList (); //Checking if the game is over
			if (totalPlayersAlive.Count == 1) {
				if (GameEnded != null)
					GameEnded.Invoke (this, new EventArgs ());
			}
		}

		/// <summary>
		/// Method is called once, at the beggining of the game.
		/// </summary>
		public void StartGame ()
		{
			if (GameStarted != null)
				GameStarted.Invoke (this, new EventArgs ());

			if (CurrentPlayerNumber != -1) {
				Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)).ForEach (u => {
					u.OnTurnStart ();
				});
				Players.Find (p => p.PlayerNumber.Equals (CurrentPlayerNumber)).Play (this);
			}
			CellGridState = new CellGridStateUnitsSelected (this, Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)));
		}

		/// <summary>
		/// Method makes turn transitions. It is called by player at the end of his turn.
		/// </summary>
		public void EndTurn ()
		{
			SetStatusText (GameStatuses.WaitingForServer, false);

			if (Units.Select (u => u.PlayerNumber).Distinct ().Count () == 1) {
				return;
			}
			CellGridState = new CellGridStateTurnChanging (this);

			Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)).ForEach (u => {
				u.OnTurnEnd ();
			});
			CurrentPlayerNumber = -1;
	        

//	        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
//	        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
//	        {
//	            CurrentPlayerNumber = (CurrentPlayerNumber + 1)%NumberOfPlayers;
//	        }//Skipping players that are defeated.

			if (TurnEnded != null)
				TurnEnded.Invoke (this, new EventArgs ());

			if (CurrentPlayerNumber != -1) {
				Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)).ForEach (u => {
					u.OnTurnStart ();
				});
				Players.Find (p => p.PlayerNumber.Equals (CurrentPlayerNumber)).Play (this);    
			}
			CellGridState = new CellGridStateUnitsSelected (this, Units.FindAll (u => u.PlayerNumber.Equals (CurrentPlayerNumber)));
		}

		public void showCallOutForCell (Cell cell, Unit fromUnit, Unit toUnit = null)
		{
			if (callOutActive) {
				CerebroHelper.DebugLog ("returning");
				return;
			}
			callOutActive = true;

			CerebroHelper.DebugLog (cell.transform.position);
			CerebroHelper.DebugLog (cell.cellIndex);
			CerebroHelper.DebugLog (cell.MovementCost);

			CallOutValueText.text = "Tile Cost: " + cell.MovementCost;
			mCurrentCellToAcquire = cell;
			mCurrentFromUnit = fromUnit;
			mCurrentToUnit = toUnit;

			CerebroHelper.DebugLog ("Show Callout");
			StartCoroutine (showCallout ());
		}

		IEnumerator showCallout ()
		{
			yield return new WaitForSeconds (0.1f);
			CerebroHelper.DebugLog ("Showing Callout");
			callOut.SetActive (true);
		}

		public void AcquirePressed ()
		{
			if (mCurrentToUnit != null) {
				DeleteUnit (mCurrentToUnit.gameObject);
			}
			UseCoinsOf (mCurrentCellToAcquire.MovementCost);
			mCurrentFromUnit.Move (mCurrentCellToAcquire, null);
			EndTurn ();

			mCurrentCellToAcquire = null;
			callOutActive = false;
			callOut.SetActive (false);
		}

		public void CalloutBGPressed ()
		{
			CerebroHelper.DebugLog ("CALL OUT BG " + callOutActive);
			callOutActive = false;
			callOut.SetActive (false);
		}

		void Update ()
		{
			if (waitingForGameData) {
				return;
			}

			if (!LaunchList.instance.mWaitingforMovesandWorldServer) {
				if (!LaunchList.instance.mDoingWorldUpdate) {
					updateCount += Time.deltaTime;
					if (updateCount >= 2f) {
						updateCount = 0f;
						HTTPRequestHelper.instance.GetGOTWorld (currentGameID);
					}
				}
			}

			updateCoinsCntr += Time.deltaTime;
			if (updateCoinsCntr >= 0.1f) {
				updateCoinsCntr = 0f;
				if (decrementScore > 0) {
					currentScore -= decrementBy;
					decrementScore -= decrementBy;
					if (currentScore > PlayerCoins) {
						coinsText.text = "Coins: " + currentScore;
					} else {
						currentScore = LaunchList.instance.mCurrentStudent.Coins;
						coinsText.text = "Coins: " + currentScore;
					}
				} else if (currentScore != PlayerCoins) {
					currentScore = LaunchList.instance.mCurrentStudent.Coins;
					coinsText.text = "Coins: " + currentScore;
				}
			}

			string timeStr = "";
			if (LaunchList.instance.mTimer.Days > 0) {
				timeStr += LaunchList.instance.mTimer.Days + " days";			
			} else if (LaunchList.instance.mTimer.Hours > 0) {
				timeStr += LaunchList.instance.mTimer.Hours + " hours";
			} else if (LaunchList.instance.mTimer.Minutes > 0) {
				timeStr += LaunchList.instance.mTimer.Minutes + " minutes";
			} else {
				timeStr += LaunchList.instance.mTimer.Seconds + " seconds!";
			}
			timerText.text = "Time Left: " + timeStr;

			if (LaunchList.instance.mTimer.TotalSeconds <= 0) {
				transform.parent.GetComponent<GOTGame> ().BackPressed ();
			}
		}
	}
}
