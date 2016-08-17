using UnityEngine;
using System.Collections.Generic;

namespace Cerebro.WordTower {
	public class Cell {

		public int characterIndex;
		public Vector2 gridPosition;
		public int myIndex;
		public BlockScript block;
		public bool isRoot = false;
		public bool isConnectedToRoot = false;

		void Awake() {
			block = null;
		}

		bool IsNeighbouringTo(Cell otherGrid) {
			if (Mathf.Abs (this.myIndex - otherGrid.myIndex) == 1 || Mathf.Abs (this.myIndex - otherGrid.myIndex) == (int)World.instance.GetGridSize().x) {
				return true;
			}
			return false;
		}

		public List<Cell> GetAllNeighbours() {
			List<Cell> neighbours = new List<Cell> ();
			Cell up = World.instance.GetCellAtIndex (this.myIndex + (int)World.instance.GetGridSize().x);
			Cell down = World.instance.GetCellAtIndex (this.myIndex - (int)World.instance.GetGridSize().x);
			Cell left = World.instance.GetCellAtIndex (this.myIndex - 1);
			Cell right = World.instance.GetCellAtIndex (this.myIndex + 1);

			if (up != null && up.characterIndex != -1) {
				neighbours.Add (up);
			}
			if (down != null && down.characterIndex != -1) {
				neighbours.Add (down);
			}
			if (left != null && left.characterIndex != -1) {
				neighbours.Add (left);
			}
			if (right != null && right.characterIndex != -1) {
				neighbours.Add (right);
			}
			return neighbours;
		}


		public Cell GetUpNeighbour() {
			Cell up = World.instance.GetCellAtIndex (this.myIndex + (int)World.instance.GetGridSize().x);

			if (up != null && up.characterIndex != -1) {
				return up;
			}
			return null;
		}

		public Cell GetDownNeighbour() {
			Cell down = World.instance.GetCellAtIndex (this.myIndex - (int)World.instance.GetGridSize().x);

			if (down != null && down.characterIndex != -1) {
				return down;
			}
			return null;
		}

		public Cell GetRightNeighbour() {
			Cell right = World.instance.GetCellAtIndex (this.myIndex + 1);

			if (right != null && right.characterIndex != -1) {
				return right;
			}
			return null;
		}

		public Cell GetLeftNeighbour() {
			Cell left = World.instance.GetCellAtIndex (this.myIndex - 1);

			if (left != null && left.characterIndex != -1) {
				return left;
			}
			return null;
		}

		public bool IsStartOfWord() {
			Cell up = World.instance.GetCellAtIndex (this.myIndex + (int)World.instance.GetGridSize().x);
			Cell left = World.instance.GetCellAtIndex (this.myIndex - 1);

			if (up != null && up.characterIndex != -1) {
				return false;
			}
			if (left != null && left.characterIndex != -1) {
				return false;
			}
			return true;
		}
	}
}