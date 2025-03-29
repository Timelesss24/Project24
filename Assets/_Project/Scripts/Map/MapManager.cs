using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Timelesss
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        [SerializeField] GameObject[] leftroomPrefabs;
        [SerializeField] GameObject[] rightroomPrefabs;
        List<Vector3> roomJoint;

        private void Awake()
        {
            Instance = this;
            roomJoint = new List<Vector3>();
            SetLeftJointTransform();
            SetRightJointTransform();
        }

        private void Start()
        {
            CreateLeftRoom();
            CreateRightRoom();
        }

        void CreateLeftRoom()
        {
            int randRoomJoint = Random.Range(0, 2);
            Instantiate(leftroomPrefabs[0], roomJoint[randRoomJoint], Quaternion.identity);
            Instantiate(leftroomPrefabs[1], roomJoint[randRoomJoint + 2], leftroomPrefabs[1].transform.rotation);

        }

        void CreateRightRoom()
        {
            int randRoomJoint = Random.Range(0, 2);
            Instantiate(rightroomPrefabs[0], roomJoint[randRoomJoint + 4], Quaternion.identity);
            Instantiate(rightroomPrefabs[1], roomJoint[randRoomJoint + 6], Quaternion.identity);

        }

        void SetLeftJointTransform()
        {
            roomJoint.Add(new Vector3(40.94f, -6.76f, -6.55f));
            roomJoint.Add(new Vector3(81.6f, 0, -6.55f));

            roomJoint.Add(new Vector3(-17.58f, 0, 4.69f));
            roomJoint.Add(new Vector3(-57.97f, -6.76f, 4.66f));
        }

        void SetRightJointTransform()
        {
            roomJoint.Add(new Vector3(27.05f, 0, 94.18f));
            roomJoint.Add(new Vector3(-14.06f, - 6.82f, 94.37f));

            roomJoint.Add(new Vector3(-33.91f, - 6.62f, 31.24f));
            roomJoint.Add(new Vector3(7.23f, 0, 31.02f));
        }
    }
}
