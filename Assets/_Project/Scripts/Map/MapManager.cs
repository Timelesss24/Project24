using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityUtils;

namespace Timelesss
{
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] GameObject[] leftroomPrefabs;
        [SerializeField] GameObject[] rightroomPrefabs;
        List<Vector3> roomJoint;

        public List<GameObject> rooms;
        public List<GameObject> enemyPrefabs;

        NavMeshSurface navMeshSurface;

        protected override void Awake()
        {
            base.Awake();

            rooms = new List<GameObject>();
            roomJoint = new List<Vector3>();
            SetLeftJointTransform();
            SetRightJointTransform();
            navMeshSurface = GetComponent<NavMeshSurface>();
            CreateLeftRoom();
            CreateRightRoom();
        }

        private void Start()
        {
            navMeshSurface.BuildNavMesh();
            SetEnemy();
        }


        void CreateLeftRoom()
        {
            int randRoomJoint = Random.Range(0, 2);
            rooms.Add(Instantiate(leftroomPrefabs[0], roomJoint[randRoomJoint], Quaternion.identity, transform));
            rooms.Add(Instantiate(leftroomPrefabs[1], roomJoint[randRoomJoint + 2], leftroomPrefabs[1].transform.rotation, transform));

        }

        void CreateRightRoom()
        {
            int randRoomJoint = Random.Range(0, 2);
            rooms.Add(Instantiate(rightroomPrefabs[0], roomJoint[randRoomJoint + 4], Quaternion.identity,transform));
            rooms.Add(Instantiate(rightroomPrefabs[1], roomJoint[randRoomJoint + 6], Quaternion.identity, transform));

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

        void SetEnemy()
        {
            foreach(GameObject room in rooms)
            {
                int randInt = Random.Range(1,5);

                for(int i = 0; i<randInt; i++)
                {
                    int randEnemy = Random.Range(0, enemyPrefabs.Count);
                    Instantiate(enemyPrefabs[randEnemy], room.transform.GetChild(1).position, Quaternion.identity, room.transform);
                }
            }
        }
    }
}
