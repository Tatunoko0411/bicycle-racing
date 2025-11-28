using rayzngames;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    BikeController MainBike;


    [SerializeField] Slider powerSlider;
    [SerializeField]Text RapTex;

    [SerializeField]public GameObject GoalUI;

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

    public void UpdatePowerSlider(float value)
    {
        powerSlider.value = value;
    }

    public void UpdateRapTex(int rap)
    {
        RapTex.text = $"{rap}/3";
    }
}
