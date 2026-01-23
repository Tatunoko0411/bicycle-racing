using UnityEngine;

public class AuthManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveScene(string name)
    {
        Initiate.Fade(name, Color.black, 1.5f);
    }

}
