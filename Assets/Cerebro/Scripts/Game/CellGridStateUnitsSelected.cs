using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cerebro {
	class CellGridStateUnitsSelected : CellGridState
	{
		private List<Unit> _units;
		private List<Cell> _pathsInRange;
		private List<Unit> _unitsInRange;

		private Cell _unitCell;

		public CellGridStateUnitsSelected(CellGrid cellGrid, List<Unit> units) : base(cellGrid)
		{
			_units = units;
			_pathsInRange = new List<Cell>();
			_unitsInRange = new List<Unit>();
		}

		public override void OnCellClicked(Cell cell)
		{
			CerebroHelper.DebugLog ("Cell Clicked");
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

				if (_cellGrid.GetCoins () >= cell.MovementCost && !cell.isInvincible) {
				//_cellGrid.showCallOutForCell (cell, fromUnit);   // uncomment this to start the callout thing

//                    _cellGrid.UseCoinsOf(cell.MovementCost);
                    fromUnit.Move(cell, null);
                    _cellGrid.EndTurn();

                    //				CerebroHelper.DebugLog ("Coins Left " + _cellGrid.GetCoinsOf(fromUnit.PlayerNumber));
                } else {
					if (cell.isInvincible) {
						_cellGrid.SetStatusText (Cerebro.GameStatuses.isInvincible);
					} else {
						_cellGrid.SetStatusText (Cerebro.GameStatuses.noCoins);
					}
				}
			}
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
				if (_cellGrid.GetCoins () >= unit.Cell.MovementCost && !unit.Cell.isInvincible) {

                    //_cellGrid.showCallOutForCell (unit.Cell, fromUnit, unit);   // uncomment this to start the callout thing

                    _cellGrid.DeleteUnit(unit.gameObject);
//                    _cellGrid.UseCoinsOf(unit.Cell.MovementCost);
                    fromUnit.Move(unit.Cell, minPath);
                    _cellGrid.EndTurn();

                    CerebroHelper.DebugLog ("Coins Left " + _cellGrid.GetCoins());
				} else {
					if (unit.Cell.isInvincible) {
						_cellGrid.SetStatusText (Cerebro.GameStatuses.isInvincible);
					} else {
						_cellGrid.SetStatusText (Cerebro.GameStatuses.noCoins);
					}
				}
			}
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

