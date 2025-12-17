using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BikeAnimController : MonoBehaviour
{

    [SerializeField] List<Sprite> sprits;
    [SerializeField] Image image;
    int SPnum;
    int cnt;
    public int speed;
       
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        speed = 60;
        cnt = 0;
        SPnum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        cnt++;

        if (cnt >= speed)
        {
            SPnum++;

            if (SPnum >= sprits.Count)
            {
                SPnum = 0;
            }

            image.sprite = sprits[SPnum];

            cnt = 0;
        }
    }
}
