using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class PlayerConditionUI : MonoBehaviour
    {
        [SerializeField] Image hpBar;
        [SerializeField] Image mpBar;

        public void SetHPBar(float amount)
        {
            hpBar.fillAmount = amount / 100.0f;
        }

        public void SetMpBar(float amount)
        {
            mpBar.fillAmount = amount / 100.0f;
        }
    }
}
