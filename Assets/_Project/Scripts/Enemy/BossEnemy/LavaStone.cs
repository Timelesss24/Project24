using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Timelesss
{
    public class LavaStone : MonoBehaviour
    {
        BossEnemy enemy;
        Rigidbody lavaRigidbody;

        public int boundForce;

        void Start()
        {
            lavaRigidbody = GetComponent<Rigidbody>();
            enemy = GetComponentInParent<BossEnemy>();

            lavaRigidbody.AddForce(new Vector3(enemy.playerDetector.Target.position.x+3, boundForce, enemy.playerDetector.Target.position.y+3), ForceMode.Impulse);
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (enemy == null) return;

                enemy.Attack();
                Destroy(this.gameObject);
            }
            else
            {
                StartCoroutine(DestroyDelay(5));
            }
        }
        IEnumerator DestroyDelay(float count)
        {
            yield return new WaitForSeconds(count);
            Destroy(this.gameObject);
        }
        
    }
}
