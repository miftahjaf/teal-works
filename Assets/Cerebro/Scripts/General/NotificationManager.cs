using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using UrbanAirship;

namespace Cerebro
{
	public class NotificationManager : MonoBehaviour 
	{

		private bool isTokenSent;

		void Start () 
		{
			UAirship.Shared.UserNotificationsEnabled = true;
			UAirship.Shared.OnPushReceived += OnPushReceived;
			UAirship.Shared.OnChannelUpdated += OnChannelUpdated;
			NotificationServices.RegisterForNotifications(
				NotificationType.Alert | 
				NotificationType.Badge | 
				NotificationType.Sound);			
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
//					PlayerPrefs.SetString (PlayerPrefKeys.DeviceToken, hexToken);
					isTokenSent = true;
				}
			}
			#endif
		}

		void OnDestroy ()
		{
			UAirship.Shared.OnPushReceived -= OnPushReceived;
			UAirship.Shared.OnChannelUpdated -= OnChannelUpdated;
		}

		public void OnDeviceTokenSent(bool isSuccess)
		{
			if (isSuccess) {
				Debug.Log ("token sent successfully");
			} else {
				Debug.Log ("token not sent");
			}
		}

		void OnChannelUpdated(string channelId) {
			Debug.Log ("Channel updated: " + channelId);
			PlayerPrefs.SetString (PlayerPrefKeys.DeviceToken, channelId);
			HTTPRequestHelper.instance.SendUrbanDeviceToken (PlayerPrefs.GetString (PlayerPrefKeys.DeviceToken));
		}

		void OnPushReceived(PushMessage message) {
			Debug.Log ("Received push! " + message.Alert);
//			#if UNITY_IOS && !UNITY_EDITOR
//			_ShowNativeAlert ("", message.Alert);
//			#endif
		}

		[DllImport ("__Internal")]
		private static extern void _ShowNativeAlert (string title, string message);
	}
}
