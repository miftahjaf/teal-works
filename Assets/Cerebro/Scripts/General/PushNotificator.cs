using UnityEngine;
using System.Collections;

namespace Cerebro {
	public class PushNotificator : MonoBehaviour {
		// Use this for initialization
		void Start () {
			//C37C6-604D6 Production
			//37FBC-E7935 Development
			//BD3DB-D495C Enterprise Production
			Pushwoosh.ApplicationCode = "BD3DB-D495C";
	//		Pushwoosh.GcmProjectNumber = "ENTER_GOOGLE_PROJECT_NUMBER_HERE";
			Pushwoosh.Instance.OnRegisteredForPushNotifications += OnRegisteredForPushNotifications;
			Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += OnFailedToRegisteredForPushNotifications;
			Pushwoosh.Instance.OnPushNotificationsReceived += OnPushNotificationsReceived;
			Pushwoosh.Instance.RegisterForPushNotifications ();
		}

		void OnRegisteredForPushNotifications(string token)
		{
			//do handling here
			CerebroHelper.DebugLog("Pushwoosh HWID" + Pushwoosh.Instance.HWID);
			CerebroHelper.DebugLog("Pushwoosh Token" + Pushwoosh.Instance.PushToken);
			CerebroHelper.DebugLog("Received token: \n" + token);
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET");
				return;
			}
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			if (Pushwoosh.Instance.HWID != "Unsupported platform") {
				LaunchList.instance.PushTheWoosh (studentID, Pushwoosh.Instance.HWID, Pushwoosh.Instance.PushToken);
			}
		}

		void OnFailedToRegisteredForPushNotifications(string error)
		{
			//do handling here
			CerebroHelper.DebugLog("Error ocurred while registering to push notifications: \n" + error);
		}

		void OnPushNotificationsReceived(string payload)
		{
			//do handling here
			CerebroHelper.DebugLog("Received push notificaiton: \n" + payload);
		}
	}
}