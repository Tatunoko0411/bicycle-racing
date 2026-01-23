using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    List<Car> carList = new List<Car>();

    int phase = 0;

    [SerializeField]
    int count = 10;

    [SerializeField]
    GameObject carPrefab;

    [SerializeField]
    Camera closeUpCamera;

    void Update()
    {
        if (phase == 0)
        {
            CheckPoint.SetForward();

            for (int i=0; i<count; i++)
            {
                var car = Instantiate<GameObject>(carPrefab).GetComponent<Car>();
                
                carList.Add(car);

            }
            phase = 1;
        }
        else if (phase == 1)
        {
            //レース中
        }
    }

    private void LateUpdate()
    {
        if (phase == 1)
        {
            //チェックポイント通過数が多い方が上(降順)
            //通過数が同じ場合は、進行度が大きいの方が上(降順)
            var order = carList.OrderByDescending(c => c.checkCount).ThenByDescending(c => c.progress);
            int rank = 0;

            foreach (var car in order)
            {
                car.SetRank(rank++);
            }

            //クローズアップカメラ
            closeUpCamera.transform.position = carList[0].transform.position - Camera.main.transform.forward * 8.0f;
            closeUpCamera.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("逆走"))
        {
            Car.revertFlag = ! Car.revertFlag;
        }
    }
}
