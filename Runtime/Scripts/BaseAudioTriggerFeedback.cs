using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

namespace JSAM 
{
    public class BaseAudioTriggerFeedback : BaseAudioFeedback<SoundFileObject>
    {
        protected enum TriggerEvent
        {
            OnTriggerEnter,
            OnTriggerStay,
            OnTriggerExit
        }

        [Header("Trigger Settings")]
        [SerializeField]
        [Tooltip("Will only play sound on trigger with another object on these layers")]
        protected LayerMask triggersWith = 0;

        [SerializeField]
        [Tooltip("The intersection event that triggers the sound to play")]
        protected TriggerEvent triggerEvent = TriggerEvent.OnTriggerEnter;
    }
}