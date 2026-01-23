using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
            //rb.AddForce(new Vector3(0,0,1));

        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed);
           // rb.AddForce(new Vector3(0,0,-1));
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
           // rb.AddForce(new Vector3(1,0,0));
        }
        if ((Input.GetKey(KeyCode.A)))
        {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            //rb.AddForce(new Vector3(-1,0,0));
        }
    }
}
