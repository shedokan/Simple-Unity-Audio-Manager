using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

namespace JSAM 
{
    [AddComponentMenu("AudioManager/Audio Trigger Feedback 2D")]
    public class AudioTriggerFeedback2D : BaseAudioTriggerFeedback
    {
        private void TriggerSound(Collider2D collision)
        {
            if (triggersWith.Contains(collision.gameObject.layer))
            {
                AudioManager.PlaySound(audio, transform);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (triggerEvent == TriggerEvent.OnTriggerEnter) TriggerSound(collision);
        }
        
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (triggerEvent == TriggerEvent.OnTriggerStay) TriggerSound(collision);
        }
        
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (triggerEvent == TriggerEvent.OnTriggerExit) TriggerSound(collision);
        }
    }
}