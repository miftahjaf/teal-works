using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
namespace Cerebro
{
	public class YoutubeCell : EnhancedScrollerCellView
	{
		public Text titleText;
		public Image thumbnail;
		public VideoData youtubeVideoData;

		public void OnPressed()
		{
			Debug.Log ("Video Id "+youtubeVideoData.videoId);
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl ("https://www.youtube.com/watch?v=" + youtubeVideoData.videoId);

		}

		void CloseWebView(object sender, System.EventArgs e) {

			CerebroHelper.DebugLog ("CLOSE WEBVIEW");
			VideoHelper.instance.VideoEnded -= CloseWebView;
			VideoEventArgs eventArgs = e as VideoEventArgs;
			Cerebro.LaunchList.instance.SendYoutubeAnalytics ("you_tube_student_log", System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), youtubeVideoData.title, youtubeVideoData.videoId, eventArgs.timeStarted, eventArgs.timeEnded);
		}
			

		public void SetYoutubeData(VideoData _youtubeVideoData)
		{
			youtubeVideoData = _youtubeVideoData;
			titleText.text = youtubeVideoData.title;
			thumbnail.color = youtubeVideoData.thumbnailSprite == null ? CerebroHelper.HexToRGB ("BA231F") : Color.white;
			thumbnail.sprite = youtubeVideoData.thumbnailSprite == null ? null : youtubeVideoData.thumbnailSprite;
		
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
			if(youtubeVideoData.thumbnailSprite == null)
			{
				string path = youtubeVideoData.thumbnailUrl;
				WWW www = new WWW (path);
				yield return www;
				if (www.error == null) {
					youtubeVideoData.thumbnailSprite = Sprite.Create(www.texture, new Rect(0, 0,www.texture.width,www.texture.height), new Vector2(0, 0));
				}	
			}
			thumbnail.sprite = youtubeVideoData.thumbnailSprite;
			thumbnail.color = youtubeVideoData.thumbnailSprite == null ? CerebroHelper.HexToRGB ("BA231F") : Color.white;
		}

		public override void RefreshCellView()
		{

		}
			

	}
}
