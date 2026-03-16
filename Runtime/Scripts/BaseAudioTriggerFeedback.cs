using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

namespace JSAM 
{
    public class BaseAudioTriggerFeedback : BaseAudioFeedback<SoundFileObject>
    {
        enum TriggerEvent
        {
            OnTriggerEnter,
            OnTriggerStay,
            OnTriggerExit
        }

        [Header("Trigger Settings")]
        [SerializeField]
        [Tooltip("Will only play sound on trigger with another object on these layers")]
        LayerMask triggersWith = 0;

        [SerializeField]
        [Tooltip("The intersection event that triggers the sound to play")]
        TriggerEvent triggerEvent = TriggerEvent.OnTriggerEnter;

        void TriggerSound(Collider other)
        {
            if (triggersWith.Contains(other.gameObject.layer))
            {
                AudioManager.PlaySound(audio, transform);
            }
        }

        void TriggerSound(Collider2D collision)
        {
            if (triggersWith.Contains(collision.gameObject.layer))
            {
                AudioManager.PlaySound(audio, transform);
            }
        }
    }
}