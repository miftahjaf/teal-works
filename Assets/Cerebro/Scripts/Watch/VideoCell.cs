using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using System;
namespace Cerebro
{
	public class VideoCell : EnhancedScrollerCellView
	{
		public Text titleText;
		public Image thumbnail;
		public Image watchedIcon;

		private VideoData videoData;
		private Action<int> onItemClick;
		private int index;

		public void OnPressed()
		{
			if (onItemClick != null) {
				onItemClick.Invoke (index);
			}
		}


		public void SetData(VideoData _videoData, int _index, Action<int> _onItemClick )
		{
			videoData = _videoData;
			index = _index;
			onItemClick = _onItemClick;

			titleText.text = videoData.title;
			thumbnail.color = videoData.thumbnailSprite == null ? Color.black : Color.white;
			thumbnail.sprite = videoData.thumbnailSprite == null ? null : videoData.thumbnailSprite;
			UpdateWatchedIcon ();

			Invoke ("LoadThumbnail", 0.01f);
		}

		public void LoadThumbnail()
		{
			if (!this.transform.parent.gameObject.activeSelf) {
				return;
			}
			StartCoroutine ("LoadThumbnailImage");
		}

		public IEnumerator LoadThumbnailImage()
		{
			if(videoData.thumbnailSprite == null)
			{
				string path = videoData.thumbnailUrl;
				WWW www = new WWW (path);
				yield return www;
				if (www.error == null) {
					videoData.thumbnailSprite = Sprite.Create(www.texture, new Rect(0, 0,www.texture.width,www.texture.height), new Vector2(0, 0));
				}	
			}
			thumbnail.sprite = videoData.thumbnailSprite;
			thumbnail.color = videoData.thumbnailSprite == null ? Color.black : Color.white;
		}

		public override void RefreshCellView()
		{
			UpdateWatchedIcon ();
		}

		public void UpdateWatchedIcon()
		{
			watchedIcon.color = new Color (watchedIcon.color.r, watchedIcon.color.g, watchedIcon.color.b, videoData.isWatched ? 1f : 0f);
		}
			
	}
}
