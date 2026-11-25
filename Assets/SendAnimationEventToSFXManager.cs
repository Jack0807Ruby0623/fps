using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class SendAnimationEventToSFXManager : MonoBehaviour
{
    public PlayerPhotonSoundManager playerPhotonSoundManager; // Unchanged

    public void TriggerFootstepSFX()
    {
        playerPhotonSoundManager.PlayFootstepSFX();
    }
}
