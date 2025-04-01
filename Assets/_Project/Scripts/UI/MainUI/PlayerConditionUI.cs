using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

namespace Timelesss
{
    public class PlayerConditionUI : MonoBehaviour
    {
        [SerializeField] Slider hpBar;
        [SerializeField] Slider staminaBar;
        [SerializeField] Slider ExpBar;
        [SerializeField] TextMeshProUGUI levelText;

        public void SetHPBar(float amount)
        {
            hpBar.value = amount / 100.0f;
        }

        public void SetStaminaBar(float amount)
        {
            staminaBar.value = amount / 100.0f;
        }

        public void SetExpBar(float amount)
        {
            ExpBar.value = amount / 100.0f;
        }

        public void SetLevel(int level)
        {
            levelText.text = level.ToString();
        }
    }
}
