using rayzngames;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Vector3 offSet;
    public Transform target;
    public float distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = target.position + (target.forward * distance)+ offSet;
      this.transform.position = pos;
      this.transform.LookAt(target);

        this.transform.rotation = new Quaternion(0,this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
    }
}
