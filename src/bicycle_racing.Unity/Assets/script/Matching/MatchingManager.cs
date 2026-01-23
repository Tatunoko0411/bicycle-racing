using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{
    
    NetWorkManager netWorkManager;

    [SerializeField]public List<Transform> PlayerPos;

    public bool isHost;

    public float waitTime;

    [SerializeField] Text CountText;

    bool isExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        if (netWorkManager != null)
        {
            netWorkManager.JoinRoom(GameManager.StageId);
            netWorkManager.ChangeState(FriendObject.State.Matching);
        }

        isExit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHost)
        {
            if (!CountText.gameObject.activeSelf)
            {
                CountText.gameObject.SetActive(true);
            }

            waitTime += Time.deltaTime;
            if (waitTime > 30)
            {
                netWorkManager.StartGame(GameManager.StageId);
                enabled = false;
                SetCount(-1);
            }

            SetCount(30 - (int)waitTime);
        }
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

    public void SetCount(int Count)
    {
        if(Count <= -1)
        {
            CountText.text = "ƒQ[ƒ€‚ðŠJŽn‚µ‚Ü‚·";
        }

        CountText.text = Count.ToString();
    }

    public void ExitMatching()
    {
        if (!isExit)
        {
            netWorkManager.LeaveRoom();
            isExit = true;
        }
        Initiate.Fade("TitleScene", Color.black, 1.5f);
    }
}
