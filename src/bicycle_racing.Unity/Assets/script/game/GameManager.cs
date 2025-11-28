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
    public bool isStart;
    float WaitTime;
    float MaxWaitTime = 3.0f;

    List<BikeController> bikeControllers = new List<BikeController>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isStart = false;
        WaitTime = MaxWaitTime;
        StartCoroutine(CountDown());

        CheckPoint.SetForward();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in objs)
        {
            BikeController bike = obj.GetComponent<BikeController>();

            if (bike != null)
            {
                bikeControllers.Add(bike);
            }
        }

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
                car.SetRank(rank++);
            }
        }

    }

    IEnumerator CountDown()
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


