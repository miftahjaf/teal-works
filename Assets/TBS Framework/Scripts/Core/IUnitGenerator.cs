using System.Collections.Generic;
using UnityEngine;

public interface IUnitGenerator
{
	List<Cerebro.Unit> SpawnUnits(List<Cell> cells);
}

