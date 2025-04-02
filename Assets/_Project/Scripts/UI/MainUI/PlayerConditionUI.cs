using System;
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

        void Start()
        {
            PlayerManager.Instance.PlayerIfo.InitalizedValueChanged();
        }

        public void SetHPBar(float amount)
        {
            hpBar.value = amount;
        }

        public void SetStaminaBar(float amount)
        {
            staminaBar.value = amount;
        }

        public void SetExpBar(float amount)
        {
            ExpBar.value = amount;
        }

        public void SetLevel(int level)
        {
            levelText.text = level.ToString();
        }
    }
}
