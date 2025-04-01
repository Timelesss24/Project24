using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemySound : MonoBehaviour
    {
        public AudioClip[] audioClips;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }
        public void IdleSound()
        {
            audioSource.PlayOneShot(audioClips[0]);
        }
        public void WalkSound()
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
        public void DieSound()
        {
            audioSource.PlayOneShot(audioClips[2]);
        }
        public void AttackFirstSound()
        {
            audioSource.PlayOneShot(audioClips[3]);
        }
        public void AttackSecondSound()
        {
            audioSource.PlayOneShot(audioClips[4]);
        }
        public void AttackThirdSound()
        {
            audioSource.PlayOneShot(audioClips[5]);
        }
    }
}
