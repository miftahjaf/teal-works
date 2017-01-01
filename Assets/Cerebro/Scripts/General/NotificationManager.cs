using UnityEngine;
using System.Collections;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace Cerebro
{
	public class NotificationManager : MonoBehaviour 
	{

		private bool isTokenSent;

		void Start () 
		{
			NotificationServices.RegisterForNotifications(
				NotificationType.Alert | 
				NotificationType.Badge | 
				NotificationType.Sound);
			PlayerPrefs.SetString (PlayerPrefKeys.DeviceToken, "0E897304089267975E76B9FEB6BFC81FF2F08AA774988B77BF0E59BA0E1FC07E");
		}

		void Update () {
			#if !UNITY_EDITOR
			if (!isTokenSent) {
				byte[] token = NotificationServices.deviceToken;
				if (token != null) {
					// send token to a provider
					string hexToken = System.BitConverter.ToString(token).Replace("-", "");
					Debug.Log ("got token "+hexToken);
					Debug.Log ("token " + token.ToString());
					PlayerPrefs.SetString (PlayerPrefKeys.DeviceToken, hexToken);
					isTokenSent = true;
				}
			}
			#endif
		}

		public void OnDeviceTokenSent(bool isSuccess)
		{
			if (isSuccess) {
				Debug.Log ("token sent successfully");
			} else {
				Debug.Log ("token not sent");
			}
		}
	}
}
