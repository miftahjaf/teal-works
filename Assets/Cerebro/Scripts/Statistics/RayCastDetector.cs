using UnityEngine;
using System.Collections;
namespace Cerebro
{
	public class RayCastDetector : MonoBehaviour
	{
	   
	
		void Update () {
			
			if (Input.GetMouseButtonUp(0)) {
				RaycastHit2D hitInfo = Physics2D.Raycast (Input.mousePosition,Vector3.one*1000f);
				if (hitInfo.collider != null) {

						hitInfo.collider.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver); //Mouse down
				}
			}
		}

	}
}
