using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public GameObject Player;
    }
}
