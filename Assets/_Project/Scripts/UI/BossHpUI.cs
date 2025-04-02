using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Timelesss
{
    public class BossHpUI : MonoBehaviour
    {
        [SerializeField] Slider bossHpBar;

        public void SetBossHpBar(float amount)
        {
            Debug.Log("보스 체력 감소");
            bossHpBar.value = amount;
        }
    }
}
