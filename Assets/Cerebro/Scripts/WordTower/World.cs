using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Cerebro.WordTower
{
	public class World : MonoBehaviour
	{
		[SerializeField]
		GameObject block;
		[SerializeField]
		GameObject WarningPopup;
		[SerializeField]
		GameObject BuyPopup;
		[SerializeField]
		GameObject staticContainer;
		[SerializeField]
		Text heightText;
		[SerializeField]
		string startWord;

		public GameObject movableContainer;
		public GameObject blockContainer;
		public GameObject trayBG;

		List<Cell> cells;

		List<BlockScript> TrayCharacters;

		Vector2 blockSize;
		Vector2 traySize;
		Vector2 minMultipleValue;
		Vector2 maxMultipleValue;
		int maxCharactersInTray = 10;

		List<int> validIndices;
		List<int> invalidIndices;
		List<int> blastCells;

		bool isCalculatingWorld = false;

		int coins = 10000;
		int wildCardBonuses = 0;

		int bonusWordLength = 5;

		List<string> bonusWords;

		float worldOffsetX = 0;

		public string gamePlayFile;

		private static World m_Instance;

		public static World instance {
			get {
				return m_Instance;
			}
		}

		void Awake ()
		{
			if (m_Instance != null && m_Instance != this) {
				Destroy (gameObject);
				return;
			}
			m_Instance = this;
		}

		// Use this for initialization
		void Start ()
		{
			gamePlayFile = Application.persistentDataPath + "/WordTowerGameplay.txt";
			blockSize = block.GetComponent<RectTransform> ().sizeDelta;
			traySize = trayBG.GetComponent<RectTransform> ().sizeDelta;

			validIndices = new List<int> ();
			invalidIndices = new List<int> ();
			bonusWords = new List<string> ();

			SetCoins ();
			StartGame ();
		}

		public void SetCoins() {
			int _coins = 0;
			if (PlayerPrefs.HasKey (PlayerPrefKeys.Coins)) {
				_coins = PlayerPrefs.GetInt (PlayerPrefKeys.Coins);
			}
			coins = _coins;
		}

		void StartGame ()
		{
			EmptyFile ();
			bonusWords.Clear ();

			movableContainer.GetComponent<RectTransform> ().localPosition = new Vector2 (movableContainer.GetComponent<RectTransform> ().localPosition.x, -768 / 2);
			TrayCharacters = new List<BlockScript> ();
			cells = new List<Cell> ();

			heightText.text = "1";
			SetInitialTray ();
			SetGrid ();
			SetInitialWord ();
			SetWorldHeight (768);
		}

		public void SetWorldHeight (float value)
		{
			if (value < 768) {
				value = 768;
			}
			movableContainer.GetComponent<RectTransform> ().sizeDelta = new Vector2 (movableContainer.GetComponent<RectTransform> ().sizeDelta.x, value);
			SetGrid ();
		}

		void SetGrid ()
		{
			var height = movableContainer.GetComponent<RectTransform> ().sizeDelta.y;
			var width = movableContainer.GetComponent<RectTransform> ().sizeDelta.x;

			var maxX = (int)Mathf.Floor ((width - blockSize.x) / blockSize.x);
			var maxY = (int)Mathf.Floor ((height - traySize.y - blockSize.y) / blockSize.y);

			minMultipleValue = new Vector2 (0f, 0f);
			maxMultipleValue = new Vector2 (maxX, maxY);
			if (cells == null) {
				cells = new List<Cell> ();
			}

			worldOffsetX = (1024 - (((int)1024 / (int)blockSize.x) * blockSize.x)) / 2;
			int k = 0;
			for (var i = 0; i <= maxY; i++) {
				for (var j = 0; j <= maxX; j++) {
					if (k >= cells.Count) {
						Cell grid = new Cell ();
						grid.gridPosition = new Vector2 (worldOffsetX + blockSize.x * j, traySize.y + blockSize.y * i);
						grid.characterIndex = -1;
						grid.myIndex = k;
						cells.Add (grid);
					}
					k++;
				}
			}
		}

		void SetInitialWord ()
		{
			var startX = (int)(maxMultipleValue.x + 1) / 2 - (int)startWord.Length / 2;
			for (var i = 0; i < startWord.Length; i++) {
				GameObject startblock = Instantiate (block);
				int charIndex = Constants.characters.IndexOf (startWord [i].ToString ());
				startblock.GetComponent<BlockScript> ().Initialize (charIndex, false);
				startblock.transform.SetParent (blockContainer.transform, false);
				startblock.transform.localPosition = cells [startX + i].gridPosition;
				cells [startX + i].block = startblock.GetComponent<BlockScript> ();
				cells [startX + i].characterIndex = charIndex;
				if (i == 0) {
					cells [startX + i].isRoot = true;
				}
			}
		}

		public void BuyButtonPressed ()
		{
			if (isCalculatingWorld) {
				return;
			}
			if (blastCells != null) {
				blastCells.Clear ();
			}
			blastCells = CheckWorld (true);
			if (blastCells.Count != 0) {
				GameObject warning = Instantiate (WarningPopup);
				warning.transform.SetParent (staticContainer.transform, false);
				warning.transform.localScale = new Vector2 (1, 1);
				warning.transform.localPosition = new Vector2 (-512, -384);
				warning.GetComponent<WarningScript> ().ContinuePressedEvent += WarningContinuePressed;
			} else {
				ShowBuyPopup ();
			}
		}

		void ShowBuyPopup ()
		{
			isCalculatingWorld = false;
			GameObject popup = Instantiate (BuyPopup);
			popup.transform.SetParent (staticContainer.transform, false);
			popup.transform.localScale = new Vector2 (1, 1);
			popup.transform.localPosition = new Vector2 (-512, -384);
			popup.GetComponent<BuyScript> ().Initialize (coins, maxCharactersInTray - TrayCharacters.Count, wildCardBonuses);
			popup.GetComponent<BuyScript> ().LettersBought += LettersBought;
		}

		private void WarningContinuePressed (object sender, EventArgs e)
		{
//		isCalculatingWorld = true;
			while (blastCells.Count != 0) {
				for (var i = 0; i < blastCells.Count; i++) {
					StartCoroutine (BlastBlock (blastCells [i]));
				}
				blastCells = CheckWorld (true);
			}
			CheckTowerHeight ();
//		StartCoroutine (ShowBuyPopupInSeconds (1.0f));
		}

		IEnumerator ShowBuyPopupInSeconds (float seconds)
		{
			yield return new WaitForSeconds (seconds);
			ShowBuyPopup ();
		}

		private void LettersBought (object sender, EventArgs e)
		{
			int increment = (e as LettersBoughtEventArgs).Coins - coins;
			(sender as BuyScript).LettersBought -= LettersBought;
			Destroy ((sender as BuyScript).gameObject);
			coins = (e as LettersBoughtEventArgs).Coins;
			List<int> lettersBought = (e as LettersBoughtEventArgs).Letters;
			wildCardBonuses = (e as LettersBoughtEventArgs).wildcardLettersLeft;
			AddCharactersToTray (lettersBought);

			LaunchList.instance.SetCoins ( increment);
		}

		void AddCharactersToTray (List<int> characters)
		{

			var characterTrayWidth = maxCharactersInTray * blockSize.x;
			var offsetX = 512 - (characterTrayWidth / 2);

			for (var i = 0; i < TrayCharacters.Count; i++) {
				TrayCharacters [i].transform.localPosition = new Vector2 (offsetX + (blockSize.x * i), 5);
			}

			for (var i = 0; i < characters.Count; i++) {
				GameObject trayBlock = Instantiate (block);
				trayBlock.GetComponent<BlockScript> ().Initialize (characters [i]);
				trayBlock.transform.SetParent (trayBG.transform, false);
				trayBlock.transform.localPosition = new Vector2 (offsetX + (blockSize.x * (TrayCharacters.Count)), 5);

				TrayCharacters.Add (trayBlock.GetComponent<BlockScript> ());
			}
		}

		public void RemoveFromTray (BlockScript obj)
		{
			TrayCharacters.Remove (obj);
		}

		void SetInitialTray ()
		{
		
			List<int> initialCharacters = new List<int> ();
			for (var i = 0; i < maxCharactersInTray; i++) {
				int toAdd = UnityEngine.Random.Range (0, Constants.characters.Count);
				if (i < maxCharactersInTray * 0.2) {
					toAdd = Constants.vowels [UnityEngine.Random.Range (0, Constants.vowels.Count)];
				}
				initialCharacters.Add (toAdd);
			}

			AddCharactersToTray (initialCharacters);
		}

		public void ResetCellAtIndex (int index)
		{
			if (index >= 0 && index < cells.Count) {
				AddMoveToFile (index, cells [index].characterIndex, false);
				cells [index].characterIndex = -1;
				cells [index].block = null;
			}
		}

		public Cell GetCellAtIndex (int index)
		{
			if (index >= 0 && index < cells.Count) {
				return cells [index];
			}
			return null;
		}

		public int GetClosestGrid (Vector2 blockPosition)
		{
			var diffY = movableContainer.GetComponent<RectTransform> ().localPosition.y + 384;

			blockPosition = new Vector2 (blockPosition.x, blockPosition.y - diffY);

			int multipleX = Mathf.RoundToInt ((blockPosition.x - worldOffsetX) / blockSize.x);
			int multipleY = Mathf.RoundToInt ((blockPosition.y - traySize.y) / blockSize.y);

			if (multipleX < (int)minMultipleValue.x) {
				multipleX = (int)minMultipleValue.x;
			} else if (multipleX > (int)maxMultipleValue.x) {
				multipleX = (int)maxMultipleValue.x;
			}

			if (multipleY < (int)minMultipleValue.y) {
				multipleY = (int)minMultipleValue.y;
			} else if (multipleY > (int)maxMultipleValue.y) {
				multipleY = (int)maxMultipleValue.y;
			}

			int index = multipleY * ((int)maxMultipleValue.x + 1) + multipleX;
			if (cells [index].characterIndex != -1) {
				return -1;
			}

			if (IsGridEmpty ()) {
				index = multipleX;
			}
			return index;
		}

		public bool IsGridEmpty ()
		{
			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1) {
					return false;
				}
			}
			return true;
		}

		public List<int> CheckWorld (bool blastBool)
		{
			MarkAllCellsConnectedToRoot ();

			validIndices.Clear ();
			invalidIndices.Clear ();

			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1 && !cells [i].isConnectedToRoot) {
					if (!cells [i].isConnectedToRoot) {
						cells [i].block.gameObject.GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f,0.5f); // make red
					}
					continue;
				}
				if (cells [i].characterIndex != -1) {
					if (cells [i].GetLeftNeighbour () == null) {
						List<int> wordChars = GetWord (i, false, new List<int> ());
						string word = "";
						for (int k = 0; k < wordChars.Count; k++) {
							word = word + Constants.characters [cells [wordChars [k]].characterIndex];
						}
						if (word.Length != 1) {
							if (!WordChecker.instance.CheckWord (word)) {
								for (int k = 0; k < wordChars.Count; k++) {
									invalidIndices.Add (wordChars [k]);
								}
							} else {
								if (word.Length > bonusWordLength && !bonusWords.Contains (word)) {
									wildCardBonuses += 1;
									bonusWords.Add (word);
									CerebroHelper.DebugLog ("Bonus received for word " + word);
								}
								for (int k = 0; k < wordChars.Count; k++) {
									validIndices.Add (wordChars [k]);
									if (cells [wordChars [k]].block.isMovable) {
										cells [wordChars [k]].block.gameObject.GetComponent<Image> ().color = Color.white;
									} else {
										cells [wordChars [k]].block.gameObject.GetComponent<Image> ().color = new Color (0.85f, 0.85f, 0.85f);
									}
								}
							}
						}
					}
					if (cells [i].GetUpNeighbour () == null) {
						List<int> wordChars = GetWord (i, true, new List<int> ());
						string word = "";
						for (int k = 0; k < wordChars.Count; k++) {
							word = word + Constants.characters [cells [wordChars [k]].characterIndex];
						}
						if (word.Length != 1) {
							if (!WordChecker.instance.CheckWord (word)) {
								for (int k = 0; k < wordChars.Count; k++) {
									invalidIndices.Add (wordChars [k]);
								}
							} else {
								if (word.Length > bonusWordLength && !bonusWords.Contains (word)) {
									wildCardBonuses += 1;
									bonusWords.Add (word);
									CerebroHelper.DebugLog ("Bonus received for word " + word);
								}
								for (int k = 0; k < wordChars.Count; k++) {
									validIndices.Add (wordChars [k]);
									if (cells [wordChars [k]].block.isMovable) {
										cells [wordChars [k]].block.gameObject.GetComponent<Image> ().color = Color.white;
									} else {
										cells [wordChars [k]].block.gameObject.GetComponent<Image> ().color = new Color (0.85f, 0.85f, 0.85f);
									}
								}
							}
						}
					}
				}
			}

			for (int i = invalidIndices.Count - 1; i >= 0; i--) {
				if (cells [invalidIndices [i]].block.isMovable) {
					cells [invalidIndices [i]].block.gameObject.GetComponent<Image> ().color = new Color (1.0f, 0.85f, 0.85f); // make red
				} else {
					invalidIndices.RemoveAt (i);
				}
			}

//		for (int i = invalidIndices.Count - 1; i >= 0; i--) {
//			if (!validIndices.Contains (invalidIndices [i])) {
//				cells [invalidIndices [i]].block.gameObject.GetComponent<Image> ().color = new Color (1.0f, 0.85f, 0.85f); // make red
//			} else {
//				if (cells [invalidIndices [i]].block.isMovable) {
//					cells [invalidIndices [i]].block.gameObject.GetComponent<Image> ().color = Color.white;
//				} else {
//					cells [invalidIndices [i]].block.gameObject.GetComponent<Image> ().color = new Color (0.85f, 0.85f, 0.85f);
//				}
//				invalidIndices.RemoveAt (i);
//			}
//		}

			return invalidIndices;
		}

		public void CheckTowerHeight ()
		{
		
			MarkAllCellsConnectedToRoot ();

			int maxHeight = 0;

			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1) {
					if (cells [i].isConnectedToRoot) {
						if (maxHeight < i) {
							maxHeight = i;
						}
					}
				}
			}
			heightText.text = (((int)maxHeight / (int)maxMultipleValue.x) + 1).ToString ();
			float towerht = cells [maxHeight].block.transform.localPosition.y;
			SetWorldHeight (towerht + 708);

		}

		IEnumerator BlastBlock (int index)
		{
			AddMoveToFile (index, cells [index].characterIndex, false);

			cells [index].block.gameObject.GetComponent<Image> ().color = new Color (242f / 255f, 38f / 255f, 19f / 255f);
			Go.to (cells [index].block.gameObject.transform, 1.0f, new GoTweenConfig ().shake (new Vector3 (10, 10, 0), GoShakeType.Position));
			cells [index].block.gameObject.transform.SetAsLastSibling ();
			cells [index].characterIndex = -1;
			yield return new WaitForSeconds (1.0f);
			if (cells [index].block != null) {
				cells [index].block.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 300;
				cells [index].block.toDestroy = true;
			}
		}

		public void PrintWorld ()
		{
			// CerebroHelper.DebugLog all characters
			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1) {
					CerebroHelper.DebugLog (i + "," + Constants.characters [cells [i].characterIndex]);
				}
			} 
		}

		public void PrintWords ()
		{
			MarkAllCellsConnectedToRoot ();

			for (var i = 0; i < cells.Count; i++) {
				if (!cells [i].isConnectedToRoot) {
					continue;
				}
				if (cells [i].characterIndex != -1) {
					if (cells [i].GetLeftNeighbour () == null) {
						List<int> wordChars = GetWord (i, false, new List<int> ());
						string word = "";
						for (int k = 0; k < wordChars.Count; k++) {
							word = word + Constants.characters [cells [wordChars [k]].characterIndex];
						}
						if (word.Length != 1) {
							CerebroHelper.DebugLog (word + "," + WordChecker.instance.CheckWord (word));
						}
					}
					if (cells [i].GetUpNeighbour () == null) {
						List<int> wordChars = GetWord (i, true, new List<int> ());
						string word = "";
						for (int k = 0; k < wordChars.Count; k++) {
							word = word + Constants.characters [cells [wordChars [k]].characterIndex];
						}
						if (word.Length != 1) {
							CerebroHelper.DebugLog (word + "," + WordChecker.instance.CheckWord (word));
						}
					}
				}
			}
		}

		List<int> GetWord (int index, bool goDown, List<int> indices)
		{			//true to go down, false to go right
//		string newWord = currentWord + Constants.characters [cells [index].characterIndex];
			indices.Add (index);
			if (goDown) {
				Cell downNeighbour = cells [index].GetDownNeighbour ();
				if (downNeighbour == null) {
					return indices;
				} else {
					return GetWord (downNeighbour.myIndex, true, indices);
				}
			} else {
				Cell rightNeighbour = cells [index].GetRightNeighbour ();
				if (rightNeighbour == null) {
					return indices;
				} else {
					return GetWord (rightNeighbour.myIndex, false, indices);
				}
			}
		}

		public void MarkAllCellsConnectedToRoot ()
		{
			int rootIndex = -1;
			for (var i = 0; i < cells.Count; i++) {
				cells [i].isConnectedToRoot = false;
				if (cells [i].isRoot) {
					rootIndex = i;
				}
			}
			if (rootIndex != -1) {
				MarkConnectionsToRoot (rootIndex);
			}
		}

		public void MarkConnectionsToRoot (int index)
		{
			if (cells [index].isConnectedToRoot) {
				return;
			}
			cells [index].isConnectedToRoot = true;

			Cell downNeighbour = cells [index].GetDownNeighbour ();
			Cell rightNeighbour = cells [index].GetRightNeighbour ();
			Cell upNeighbor = cells [index].GetUpNeighbour ();
			Cell leftNeighbour = cells [index].GetLeftNeighbour ();

			if (downNeighbour != null) {
				MarkConnectionsToRoot (downNeighbour.myIndex);
			}
			if (upNeighbor != null) {
				MarkConnectionsToRoot (upNeighbor.myIndex);
			}
			if (leftNeighbour != null) {
				MarkConnectionsToRoot (leftNeighbour.myIndex);
			}
			if (rightNeighbour != null) {
				MarkConnectionsToRoot (rightNeighbour.myIndex);
			}
		}

		public Vector2 GetGridSize ()
		{
			return new Vector2 (maxMultipleValue.x + 1, maxMultipleValue.y + 1);
		}

		public void ResetGame ()
		{
			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1) {
					cells [i].characterIndex = -1;
					cells [i].block.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 300;
					cells [i].block.toDestroy = true;
					cells [i].block = null;
				}
			}
			for (var i = 0; i < TrayCharacters.Count; i++) {
				BlockScript script = TrayCharacters [i];
				Destroy (script.gameObject);
			}

			StartCoroutine (WaitForStartGame ());
		}

		public void AddMoveToFile (int cellIndex, int characterIndex, bool placed)
		{
			StreamWriter writesr;
			if (File.Exists (gamePlayFile)) {
				writesr = File.AppendText (gamePlayFile);
			} else {
				writesr = File.CreateText (gamePlayFile);
			}
			if (placed) {
				writesr.WriteLine (cellIndex + "," + characterIndex + ",1");
			} else {
				writesr.WriteLine (cellIndex + "," + characterIndex + ",0");
			}
			writesr.Close ();
		}

		void EmptyFile ()
		{
			StreamWriter writesr;
			writesr = File.CreateText (gamePlayFile);
			writesr.Close ();
		}

		public void ReplayGame ()
		{
			List<Moves> moves = new List<Moves> ();
			if (File.Exists (gamePlayFile)) {
				var sr = File.OpenText (gamePlayFile);
				var line = sr.ReadLine ();
				while (line != null) {
					var splitarr = line.Split ("," [0]);
					Moves move = new Moves ();
					move.CellIndex = int.Parse (splitarr [0]);
					move.CharacterIndex = int.Parse (splitarr [1]);
					move.Placed = int.Parse (splitarr [2]);
					moves.Add (move);
					line = sr.ReadLine ();
				}  
				StartCoroutine (ReplayMoves (moves));
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + gamePlayFile + " for reading.");
				return;
			}
		}

		IEnumerator ReplayMoves (List<Moves> moves)
		{
			for (var i = 0; i < cells.Count; i++) {
				if (cells [i].characterIndex != -1 && cells [i].block.isMovable) {
					cells [i].characterIndex = -1;
					Destroy (cells [i].block.gameObject);
					cells [i].block = null;
				}
			}

			foreach (var item in moves) {
				int cellIndex = item.CellIndex;
				int charIndex = item.CharacterIndex;
				int placed = item.Placed;

				if (placed == 1) {
					GameObject replayblock = Instantiate (block);
					replayblock.GetComponent<BlockScript> ().Initialize (charIndex, true, cellIndex);
					replayblock.transform.SetParent (blockContainer.transform);
					replayblock.transform.localPosition = cells [cellIndex].gridPosition;
					replayblock.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
					cells [cellIndex].block = replayblock.GetComponent<BlockScript> ();
					cells [cellIndex].characterIndex = charIndex;
				} else {
					Destroy (cells [cellIndex].block.gameObject);
					cells [cellIndex].characterIndex = -1;
					cells [cellIndex].block = null;
				}
				yield return new WaitForSeconds (0.1f);
			}
		}

		IEnumerator WaitForStartGame ()
		{
			heightText.text = "0";
			yield return new WaitForSeconds (2.0f);
			StartGame ();
		}

		public class Moves
		{
			public int CharacterIndex { get; set; }

			public int CellIndex{ get; set; }

			public int Placed { get; set; }
		}
	}
}