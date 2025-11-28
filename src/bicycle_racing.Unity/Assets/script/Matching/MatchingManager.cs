using UnityEngine;
using System.Collections;

public class MatchingManager : MonoBehaviour
{
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("ŠJŽn‚µ‚Ü‚·");
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartGame()
    {
        int cnt = 3;
        while (true)
        {
            yield return new WaitForSeconds(1);
            cnt--;
            Debug.Log(cnt);
            if (cnt == 0)
            {
                Initiate.Fade("GameScene",Color.black,1.5f);
                break;
            }

        }
    }

}
