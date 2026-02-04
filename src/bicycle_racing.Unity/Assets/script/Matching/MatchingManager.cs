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
    [SerializeField] Button StartButton;

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
            StartButton.gameObject.SetActive(true);
        }
    

        if (!CountText.gameObject.activeSelf)
        {
            CountText.gameObject.SetActive(true);
        }

        waitTime += Time.deltaTime;
        if (waitTime > 30)
        {
            if (isHost)
            {
                netWorkManager.StartGame(GameManager.StageId);
            }
            enabled = false;
       
            return;
        }

        SetCount(30 - (int)waitTime);
    }

    /// <summary>
    /// ゲームへ遷移
    /// </summary>
    public IEnumerator StartGame()
    {
        enabled = false ;
        int cnt = 3;
        SetCount(-1);
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

    /// <summary>
    ///カウントダウンのUI変更 
    /// </summary>
    /// <param count="Count">残り時間（-1の場合はゲーム開始通知に変える）</param>
    public void SetCount(int Count)
    {
        CountText.text = Count.ToString();
        if (Count <= -1)
        {
            CountText.text = "ゲームを開始します";
        }


    }

    /// <summary>
    /// マッチング退出
    /// </summary>
    public void ExitMatching()
    {
        if (!isExit)
        {
            netWorkManager.LeaveRoom();
            isExit = true;
        }
        Initiate.Fade("TitleScene", Color.black, 1.5f);
    }

    /// <summary>
    /// マッチング終了
    /// </summary>
    public void EndMatcing()
    {
        netWorkManager.StartGame(GameManager.StageId);
    }
}
