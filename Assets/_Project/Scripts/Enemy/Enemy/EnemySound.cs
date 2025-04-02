using System.Collections;
using System.Collections.Generic;
using Framework.Audio;
using UnityEngine;

namespace Timelesss
{
    public class EnemySound : MonoBehaviour
    {
        public AudioClip[] audioClips;
        //private AudioSource audioSource;

        private void Start()
        {
            //audioSource = GetComponent<AudioSource>();
            
        }
        public void IdleSound()
        {
            SoundManager.PlaySfx(audioClips[0]);
        }
        public void WalkSound()
        {
            SoundManager.PlaySfx(audioClips[1]);
        }
        public void DieSound()
        {
            SoundManager.PlaySfx(audioClips[2]);
        }
        public void AttackFirstSound()
        {
            SoundManager.PlaySfx(audioClips[3]);
        }
        public void AttackSecondSound()
        {
            SoundManager.PlaySfx(audioClips[4]);
        }
        public void AttackThirdSound()
        {
            SoundManager.PlaySfx(audioClips[5]);
        }
    }
}
