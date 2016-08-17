// Skeleton example for InternetReachabilityVerifier.
//
// Copyright 2014 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 840 $
//
// File version history:
// 2014-06-18, 1.0.0 - Initial version

using UnityEngine;

[RequireComponent(typeof(InternetReachabilityVerifier))]
public class SkeletonIRVExample : MonoBehaviour
{
    InternetReachabilityVerifier internetReachabilityVerifier;

    // Returns true when there is verified internet access.
    bool isNetVerified()
    {
        return internetReachabilityVerifier.status == InternetReachabilityVerifier.Status.NetVerified;
    }

    // Requests that the internet access is verified again.
    /* This is done by simply force "pending verification" status,
     * InternetReachabilityVerifier has a coroutine which will notice this.
     * (You should do this if you know some other way that
     *  network connectivity has been lost or something.)
     */
    void forceReverification()
    {
        internetReachabilityVerifier.status = InternetReachabilityVerifier.Status.PendingVerification;
    }


    // This is called when the internet access status changes.
    // Note: this delegate is set up in Start() below.
    void netStatusChanged(InternetReachabilityVerifier.Status newStatus)
    {
        Debug.Log("InternetReachabilityVerifier.Status: " + newStatus);
    }

    void Start()
    {
        internetReachabilityVerifier = GetComponent<InternetReachabilityVerifier>();
        internetReachabilityVerifier.statusChangedDelegate += netStatusChanged;

        // Note: See the CustomIRVExample for more info about how to
        //       verify Internet access using your self-hosted URL.
    }
}
