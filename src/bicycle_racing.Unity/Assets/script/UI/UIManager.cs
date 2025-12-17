using rayzngames;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    BikeController MainBike;


    [SerializeField] Slider powerSlider;
    [SerializeField] Slider speedSlider;
    [SerializeField]Text RapTex;
    [SerializeField]Text RankTex;

    [SerializeField]public GameObject GoalUI;

   [SerializeField] BikeAnimController bikeAnimController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoalUI.SetActive(false);
         

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPowerSlider(float Max)
    {
        powerSlider.maxValue = Max;
    }

    public void InitSpeedSlider(float Max)
    {
        speedSlider.maxValue = Max;
    }

    public void UpdatePowerSlider(float value)
    {
        powerSlider.value = value;
    }

    public void UpdateSpeedSlider(float value)
    {
        speedSlider.value = value;
    }

    public void UpdateRapTex(int rap)
    {
        RapTex.text = $"{rap}/3";
    }

    public void SetBikeAnimSpeed(int speed)
    {
        bikeAnimController.speed = speed;
    }

    public void SetRankText(int rnk)
    {
        RankTex.text = $"{rnk}ˆÊ";
    }
}
