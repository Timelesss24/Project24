using System.Collections;
using System.Collections.Generic;
using Framework.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundEffect : MonoBehaviour
    {
        [SerializeField] private AudioClip clickSound;

        private Button[] buttons;

        private void Awake()
        {
            buttons = GetComponents<Button>();
            foreach (var button in buttons)
            {
                button.onClick.AddListener(PlayClickSound);
            }
        }

        private void PlayClickSound()
        {
            if (clickSound == null)
                clickSound = SoundManager.Instance.ClickSound;
            
            SoundManager.PlaySfx(clickSound);
        }
    }
}