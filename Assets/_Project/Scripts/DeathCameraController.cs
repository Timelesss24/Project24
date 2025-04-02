using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFavorites.Libs;

namespace Timelesss
{
    public class DeathCameraController : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        public CinemachineSmoothPath dollyPath;
        public Transform player;
        public float moveSpeed = 0.1f;

        void Start()
        {
            virtualCamera.Priority = 0;
            PlayerManager.Instance.PlayerIfo.DeathAction += StartCameraMove;
            dollyPath.transform.position = new Vector3(0,0,0);

        }

        public void StartCameraMove()
        {
            player = PlayerManager.Instance.transform;
            virtualCamera.Priority = 20;
            Vector3 playerPos = player.position;
            dollyPath.transform.position = player.position + player.up * 5f;
        }
    }
}
