using bicycle_racing.Shared.Models.Entities;
using rayzngames;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{

    NetWorkManager netWorkManager;

    [SerializeField] List<GameObject> Maps;

    [SerializeField] public List<Transform> PlayerPos;
    [SerializeField] GameObject BikePrefab;
    int Count;

    bool isLeave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        Count = 0;

        foreach (var map in Maps)
        {
            map.SetActive(false);
        }

        Maps[GameManager.StageId - 1].SetActive(true);

        foreach (User user in netWorkManager.GoalPlayerList.Values)
        {
            SetPlayer(user);
        }
    }

    /// <summary>
    /// プレイヤーの配置
    /// </summary>
    /// <param name="user">ユーザ情報</param>
    public void SetPlayer(User user)
    {
        GameObject obj = Instantiate(BikePrefab,
            PlayerPos[Count].position,
            Quaternion.identity);

        BikeController bikeController = obj.GetComponent<BikeController>();

        bikeController.NameText.text = user.Name;

        Vector3 direction = Camera.main.transform.position - bikeController.NameText.transform.position;
        bikeController.NameText.transform.rotation = Quaternion.LookRotation(-direction);

        bikeController.gameObject.transform.localScale =new Vector3(1.5f,1.5f,1.5f);



        Count++;
    }

    /// <summary>
    /// マッチング遷移
    /// </summary>
    public void Retry()
    {
        netWorkManager.LeaveRoom();

        Initiate.Fade("MatcingManager",Color.black,1.5f);
    }

    /// <summary>
    /// タイトル遷移
    /// </summary>
    public void MoveTitle()
    {
        if (!isLeave)
        {
            isLeave =true;
            netWorkManager.LeaveRoom();
        }

        Initiate.Fade("TitleScene",Color.black,1.5f);

    }
}
