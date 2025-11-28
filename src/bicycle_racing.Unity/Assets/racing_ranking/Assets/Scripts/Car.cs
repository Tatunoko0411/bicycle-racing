using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car : MonoBehaviour
{
    //今走っているチェックポイント
    CheckPoint nowCheckPoint;

    //クリアしたチェックポイント数
    int _checkCount;
    public int checkCount => _checkCount;

    //UI表示
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    RectTransform info;

    [SerializeField]
    GameObject wrongWay;

    [SerializeField]
    Text rank;

    float speed;

    //最高・最低速度
    float minSpeed = 1.0f * 1000f / 60f;
    float maxSpeed = 10.0f * 1000f / 60f;

    float nextSpeedChangeTime;
    float nextRotChangeTime;

    Quaternion turnRot;

    static public bool revertFlag;

    void Awake()
    {
        nowCheckPoint = CheckPoint.StartPoint;
        transform.position = nowCheckPoint.transform.position;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 10;
    }
       
    void Update()
    {
        var p0 = transform.position;

        if (nextSpeedChangeTime < Time.time)
        {
            //2.0秒に一回スピードを変更する
            speed = Random.Range(minSpeed, maxSpeed);
            nextSpeedChangeTime = Time.time + 2.0f;
        }
        if (nextRotChangeTime < Time.time)
        {
            //1.0秒に一回、向きオフセットを変える
            turnRot = Quaternion.AngleAxis(Random.Range(-10f, 10f), Vector3.up);
            if (revertFlag && Random.Range(0, 2) == 0)
            {
                //逆走モード中は1/2で逆走する
                turnRot *= Quaternion.AngleAxis(180f, Vector3.up);
            }
            nextRotChangeTime = Time.time + 1.0f;
        }

        var v = nowCheckPoint.nextCheckPoint.transform.position - transform.position;
        v.y = 0;
        v = v.normalized * Time.deltaTime * speed;

        //進行方向をすこし揺らす
        v = turnRot * v;
        var p1 = p0 + v;

        transform.position = p1;
        transform.rotation = Quaternion.LookRotation(v);

        if (nowCheckPoint.CheckWrongWay(p0, p1))
        {
            //逆走
            wrongWay.SetActive(true);
        }
        else
        {
            //逆走してない
            wrongWay.SetActive(false);
        }

        if (nowCheckPoint.CheckIfPassed(p0, p1))
        {
            //チェックポイント通過
            nowCheckPoint = nowCheckPoint.nextCheckPoint;
            _checkCount++;
        }

    }

    private void LateUpdate()
    {
        //順位表示UIをカメラに向ける
        canvas.transform.rotation = Camera.main.transform.rotation;
    }

    //そのチェックポイントにおける進行度
    public float progress => nowCheckPoint.GetProgress(transform.position);

    public void SetRank(int rank)
    {
        this.rank.text = $"#{rank + 1}";
    }
}
