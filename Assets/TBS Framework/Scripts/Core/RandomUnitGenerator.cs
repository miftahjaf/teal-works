using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class RandomUnitGenerator : MonoBehaviour, IUnitGenerator
{
    private System.Random _rnd = new System.Random();

    public Transform UnitsParent;

    public GameObject UnitPrefab;
    public int NumberOfPlayers;
    public int UnitsPerPlayer;

	public List<Cerebro.Unit> SpawnUnits(List<Cell> cells)
    {
		List<Cerebro.Unit> ret = new List<Cerebro.Unit>();

        List<Cell> freeCells = cells.FindAll(h => h.GetComponent<Cell>().IsTaken == false);
        freeCells = freeCells.OrderBy(h => _rnd.Next()).ToList();

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            for (int j = 0; j < UnitsPerPlayer; j++)
            {
                var cell = freeCells.ElementAt(0);
                freeCells.RemoveAt(0);
                cell.GetComponent<Cell>().IsTaken = true;

                var unit = Instantiate(UnitPrefab);
                unit.transform.position = cell.transform.position + new Vector3(0, 0, 0);
				unit.GetComponent<Cerebro.Unit>().PlayerNumber = i;
				unit.GetComponent<Cerebro.Unit>().Cell = cell.GetComponent<Cell>();
				unit.GetComponent<Cerebro.Unit>().Initialize();
                unit.transform.parent = UnitsParent;


				ret.Add(unit.GetComponent<Cerebro.Unit>());
            }
        }
        return ret;
    }
}

