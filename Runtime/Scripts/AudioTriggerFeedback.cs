using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

namespace JSAM 
{
    [AddComponentMenu("AudioManager/Audio Trigger Feedback")]
    public class AudioTriggerFeedback : BaseAudioTriggerFeedback
    {
        private void OnTriggerEnter(Collider other)
        {
            if (triggerEvent == TriggerEvent.OnTriggerEnter) TriggerSound(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (triggerEvent == TriggerEvent.OnTriggerStay) TriggerSound(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggerEvent == TriggerEvent.OnTriggerExit) TriggerSound(other);
        }
    }
}