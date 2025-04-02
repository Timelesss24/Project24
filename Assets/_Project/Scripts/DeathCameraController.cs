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

        void Start()
        {
            PlayerManager.Instance.PlayerIfo.DeathAction += StartCameraMove;
        }

        public void StartCameraMove()
        {
            player = PlayerManager.Instance.transform;
            virtualCamera.Priority = 20;
            Vector3 playerPos = player.position;
            dollyPath.transform.position = player.position + player.up * 5f;

            dollyPath.InvalidateDistanceCache();
        }
    }
}
