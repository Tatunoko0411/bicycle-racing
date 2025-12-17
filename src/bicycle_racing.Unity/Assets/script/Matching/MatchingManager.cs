using UnityEngine;
using System.Collections;

public class MatchingManager : MonoBehaviour
{
    
    NetWorkManager netWorkManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        if (netWorkManager != null)
        {
            netWorkManager.JoinRoom(GameManager.StageId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartGame()
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
