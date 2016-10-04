using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cerebro {
	class CellGridStateUnitsSelected : CellGridState
	{
		private List<Unit> _units;
		private List<Cell> _pathsInRange;
		private List<Unit> _unitsInRange;

		private Cell _unitCell;
		private GameObject CapturePopup;

		private Unit currUnit, currUnitToDelete;
		private Cell currCell;
		private List<Cell> currMinPath;

		public CellGridStateUnitsSelected(CellGrid cellGrid, List<Unit> units) : base(cellGrid)
		{
			_units = units;
			_pathsInRange = new List<Cell>();
			_unitsInRange = new List<Unit>();
		}

		public override void OnCellClicked(Cell cell)
		{
			if (cell.transform.parent.GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup>().IsPopupEnabled && EventSystem.current.IsPointerOverGameObject ()) {
				CerebroHelper.DebugLog ("returning Cell Clicked "+cell.transform.position.y);
				return;
			}
			CerebroHelper.DebugLog ("Cell Clicked "+cell.transform.position.y);

			foreach (var _unit in _units) {
				if (_unit.isMoving) {
					return;
				}
			}
			if(cell.IsTaken)
			{
				return;
			}

			if(_pathsInRange.Contains(cell))
			{
				var minPathLength = 100;
				List<Cell> minPath = new List<Cell>();
				Unit fromUnit = null;
				foreach (var _unit in _units) {
					var path = _unit.FindPath(_cellGrid.Cells, cell);
					if (path.Count < minPathLength) {
						minPathLength = path.Count;
						minPath = path;
						fromUnit = _unit;
					}
				}
				CerebroHelper.DebugLog ("Will Cost " + cell.MovementCost + " Coins");

				if (!cell.isInvincible) {
					float pos = cell.transform.position.y;
					pos = Mathf.Abs(-0.3831128f - pos) + 0.3831128f;
					Debug.Log ("setting "+pos);
					Go.to (Camera.main.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (0f, pos, -10f), false).setEaseType (GoEaseType.BackOut));

					string BabaId = "";
					if (cell.BabaHairId > 0) {
						BabaId += cell.BabaHairId;
						BabaId += cell.BabaFaceId;
						BabaId += cell.BabaBodyId;
					}
					Debug.Log ("baba id "+BabaId);
					if (_cellGrid.GetCoins () >= cell.MovementCost) {
						cell.transform.parent.GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup> ().InitializePopup ("Capture", cell.MovementCost, BabaId, cell.groupID, OnCaptureButtonPressed, cell.transform.position, OnCancelButtonPressed);
					} else {
						cell.transform.parent.GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup> ().InitializePopup ("Capture", cell.MovementCost, BabaId, cell.groupID, OnCaptureButtonPressed, cell.transform.position, OnCancelButtonPressed, false);
					}
					cell.transform.parent.GetComponent<CellGrid> ().pointsTray.GetComponent<MaterialUI.EasyTween> ().Tween("BounceOut");
					currUnit = fromUnit;
					currCell = cell;
                } else {
					_cellGrid.SetStatusText (Cerebro.GameStatuses.isInvincible);
				}
			}
		}

		public void OnCaptureButtonPressed()
		{
			currUnit.Move(currCell, null);
			_cellGrid.EndTurn();
		}

		public void OnCancelButtonPressed()
		{
			currCell.transform.parent.GetComponent<CellGrid> ().GetBackPointsTray ();
		}

		public override void OnUnitClicked(Unit unit)
		{
			CerebroHelper.DebugLog ("Unit Clicked");
			foreach (var _unit in _units) {
				if (unit.Equals (_unit)) {
					return; // put this after the if case below, to enable strengthening yourself
					if (_cellGrid.GetCoins () >= 1) {
						_cellGrid.UseCoinsOf (1);
						unit.Cell.MovementCost += 1;
						_cellGrid.updateCellCost (unit.Cell, unit.Cell.MovementCost);
					}
				}
			}

			if (_unitsInRange.Contains (unit)) {
				var minPathLength = 100;
				List<Cell> minPath = new List<Cell> ();
				Unit fromUnit = null;
				foreach (var _unit in _units) {
					var path = _unit.FindPath (_cellGrid.Cells, unit.Cell);
					if (path.Count < minPathLength) {
						minPathLength = path.Count;
						minPath = path;
						fromUnit = _unit;
					}
				}

				CerebroHelper.DebugLog ("Will Cost " + unit.Cell.MovementCost + " Coins");
				if (!unit.Cell.isInvincible) {
					float pos = unit.Cell.transform.position.y;
					pos = Mathf.Abs(-0.3831128f - pos) + 0.3831128f;
					Debug.Log ("setting "+pos);
					Go.to (Camera.main.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (0f, pos, -10f), false).setEaseType (GoEaseType.BackOut));

					string BabaId = "";
					if (unit.Cell.BabaHairId > 0) {
						BabaId += unit.Cell.BabaHairId;
						BabaId += unit.Cell.BabaFaceId;
						BabaId += unit.Cell.BabaBodyId;
					}
					Debug.Log ("Unit baba id "+BabaId);
					if (_cellGrid.GetCoins () >= unit.Cell.MovementCost) {
						unit.Cell.transform.parent.GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup> ().InitializePopup ("Capture", unit.Cell.MovementCost, BabaId, unit.Cell.groupID, OnCaptureUnitButtonPressed, unit.Cell.transform.position, OnCancelButtonPressed);
					} else {
						unit.Cell.transform.parent.GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup>().InitializePopup ("Capture", unit.Cell.MovementCost, BabaId, unit.Cell.groupID, OnCaptureUnitButtonPressed, unit.Cell.transform.position, OnCancelButtonPressed, false);
					}
					unit.Cell.transform.parent.GetComponent<CellGrid> ().pointsTray.GetComponent<MaterialUI.EasyTween> ().Tween("BounceOut");

					currUnitToDelete = unit;
					currUnit = fromUnit;
					currCell = unit.Cell;
					currMinPath = minPath;
                    CerebroHelper.DebugLog ("Coins Left " + _cellGrid.GetCoins());
				} else {
					_cellGrid.SetStatusText (Cerebro.GameStatuses.isInvincible);
				}
			}
		}

		public void OnCaptureUnitButtonPressed()
		{
			if (currUnitToDelete) {
				_cellGrid.DeleteUnit (currUnitToDelete.gameObject);
			}
			currUnit.Move(currCell, currMinPath);
			_cellGrid.EndTurn();
		}

		public override void OnCellDeselected(Cell cell)
		{
			base.OnCellDeselected(cell);

			foreach (var _cell in _pathsInRange)
			{
				_cell.MarkAsReachable();
			}
			foreach (var _cell in _cellGrid.Cells.Except(_pathsInRange))
			{
				_cell.UnMark();
			}
		}
		public override void OnCellSelected(Cell cell)
		{
			base.OnCellSelected(cell);
			if (!_pathsInRange.Contains(cell)) return;
			var minPathLength = 100;
			List<Cell> minPath = new List<Cell>();
			foreach (var _unit in _units) {
				var path = _unit.FindPath(_cellGrid.Cells, cell);
				if (path.Count < minPathLength) {
					minPathLength = path.Count;
					minPath = path;
				}
			}
			foreach (var _cell in minPath)
			{
				_cell.MarkAsPath();
			}
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();

			foreach (var cell in _cellGrid.Cells)
			{
				cell.UnMark();
			}
				
			foreach (var _unit in _units) {
				_unit.OnUnitSelected();

				var paths = _unit.GetAvailableDestinations(_cellGrid.Cells);

				foreach (var path in paths) {
					bool isFriend = false;
					foreach (var tmpunit in _units) {
						if (tmpunit.Cell.cellIndex == path.cellIndex) {
							isFriend = true;
						}
					}
					if (!isFriend) {
						_pathsInRange.Add (path);
					}
				}
				foreach (var cell in _pathsInRange)
				{
					cell.MarkAsReachable();
				}

				_unitCell = _unit.Cell;

				if (_unit.ActionPoints <= 0) return;

				foreach (var currentUnit in _cellGrid.Units)
				{
					if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
						continue;

					if (_unit.IsUnitAttackable(currentUnit,_unit.Cell))
					{
						currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
						_unitsInRange.Add(currentUnit);
					}
				}


	//			if (_unitCell.GetNeighbours(_cellGrid.Cells).FindAll(c => c.MovementCost <= _unit.MovementPoints).Count == 0 
	//				&& _unitsInRange.Count == 0)
	//				_unit.SetState(new UnitStateMarkedAsFinished(_unit));
			}
		}

		public override void OnStateExit()
		{
//			foreach (var _unit in _units) {
//				_unit.OnUnitDeselected ();
//				foreach (var unit in _unitsInRange) {
//					if (unit == null)
//						continue;
//					unit.SetState (new UnitStateNormal (unit));
//				}
//				foreach (var cell in _cellGrid.Cells) {
//					cell.UnMark ();
//				}  
//			}
		}
	}
}

