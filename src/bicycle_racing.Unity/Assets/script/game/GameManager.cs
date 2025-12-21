using NUnit.Framework;
using rayzngames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-5)]

public class GameManager : MonoBehaviour
{
    static public int StageId = 1;

    public bool isStart;
    float WaitTime;
    float MaxWaitTime = 3.0f;
    bool allGoal;

    public List<BikeController> bikeControllers = new List<BikeController>();

    [SerializeField] UIManager uiManager;

    public List<GameObject> StartPoints = new List<GameObject>();

    NetWorkManager netWorkManager;

    [SerializeField] List<GameObject> Maps = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isStart = false;
        WaitTime = MaxWaitTime;

        foreach (GameObject map in Maps)
        {
            map.SetActive(false);
        }

        Maps[StageId-1].SetActive(true);

        CheckPoint.SetForward();

        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        netWorkManager.SetPlayers();

        netWorkManager.Ready();

        //StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            //チェックポイント通過数が多い方が上(降順)
            //通過数が同じ場合は、進行度が大きいの方が上(降順)
            
            var order = bikeControllers.OrderByDescending(c => c.checkCount).ThenByDescending(c => c.progress);
            int rank = 0;

            foreach (var car in order)
            {
                rank++;
                car.SetRank(rank);
             
            }

            allGoal = true;

            foreach (BikeController controller in bikeControllers)
            {
                if(!controller.isGoal)
                {
                    allGoal = false;
                }
            }

            if (allGoal)
            {
                uiManager.GoalUI.SetActive(true);
                Debug.Log("AllGoal");
            }
        }

    }

     public IEnumerator CountDown()
    {
        while (true)
        {
            Debug.Log(WaitTime);
            yield return new WaitForSeconds(1.0f);

            WaitTime--;

            if (WaitTime <= 0)
            {
                isStart = true;
                Debug.Log("スタート");

                break;
            }
        }
    }
}


