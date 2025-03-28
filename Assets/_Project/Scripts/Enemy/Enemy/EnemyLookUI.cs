using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class EnemyLookUI : MonoBehaviour
    {
        public Camera playerCamera;

        // Update is called once per frame
        void Update()
        {
            LookCamera();
        }

        void LookCamera()
        {
            if(playerCamera != null)
            {
                transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);
            }
        }
    }
}
