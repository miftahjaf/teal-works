class CellGridStateWaitingForInput : CellGridState
{
	public CellGridStateWaitingForInput(Cerebro.CellGrid cellGrid) : base(cellGrid)
    {
    }

	public override void OnUnitClicked(Cerebro.Unit unit)
    {
        if(unit.PlayerNumber.Equals(_cellGrid.CurrentPlayerNumber))
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit); 
    }
}
