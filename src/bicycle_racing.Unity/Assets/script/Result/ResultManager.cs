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

    public void SetPlayer(User user)
    {
        GameObject obj = Instantiate(BikePrefab,
            PlayerPos[Count].position,
            Quaternion.identity);

        BikeController bikeController = obj.GetComponent<BikeController>();

        bikeController.NameText.text = user.Name;

        Count++;
    }

    public void Retry()
    {
        netWorkManager.LeaveRoom();

        Initiate.Fade("MatcingManager",Color.black,1.5f);
    }

    public void MoveTitle()
    {
        netWorkManager.LeaveRoom();

        Initiate.Fade("TitleScene",Color.black,1.5f);

    }
}
