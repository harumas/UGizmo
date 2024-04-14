using UnityEngine;

namespace UGizmo
{
    public class ObjectMovement : MonoBehaviour
    {
        public float speed = 5.0f;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * speed * Time.deltaTime;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position,4f);
        }
    }
}