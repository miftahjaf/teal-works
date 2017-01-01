using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Cerebro {

	public class HomeworkTitleCellView : EnhancedScrollerCellView {

		public Text titleText;

		private HomeworkTitleCell mCurrTitleCell;
		public HomeworkTitleCell currTitleCell
		{
			get { return mCurrTitleCell; }
			set { mCurrTitleCell = value; }
		}

		public void InitializeCell(HomeworkTitleCell titleCell)
		{
			currTitleCell = titleCell;
			titleText.text = currTitleCell.cellTitle;
		}

		public override void RefreshCellView()
		{
			
		}

		public void OnCellClicked()
		{

		}

	}

}
