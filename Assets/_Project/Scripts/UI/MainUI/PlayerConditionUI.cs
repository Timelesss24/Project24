using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class PlayerConditionUI : MonoBehaviour
    {
        [SerializeField] Slider hpBar;
        [SerializeField] Slider staminaBar;

        [SerializeField] EventChannel<float> hpChannel;
        [SerializeField] EventChannel<float> mpChannel;

        public void SetHPBar(float amount)
        {
            hpBar.value = amount / 100.0f;
        }

        public void SetStaminaBar(float amount)
        {
            staminaBar.value = amount / 100.0f;
        }
    }
}
