using rayzngames;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    BikeController MainBike;


    [SerializeField] Slider powerSlider;
    [SerializeField] Image PowerSliderBackGround;

    [SerializeField] Slider speedSlider;

    [SerializeField]Text RapTex;
    [SerializeField]Text RankTex;

    [SerializeField]public GameObject GoalUI;

   [SerializeField] BikeAnimController bikeAnimController;
   
    [SerializeField] List<Sprite> ItemspriteList;
    [SerializeField] Image ItemButton;


    [SerializeField]public GameObject UsingItemUI;
    [SerializeField] Slider UsingItemSlider;
    [SerializeField] Image UsingItemImage;

    [SerializeField]List<Sprite> PowerSpriteList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoalUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GoalUI.activeSelf)
        {
            GoalUI.transform.position = new Vector3(GoalUI.transform.position.x + 2f,GoalUI.transform.position.y,GoalUI.transform.position.z);
        }
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

    public void ChengeItemSprite(int itemNum)
    {
        ItemButton.sprite = ItemspriteList[itemNum];

    }

    public void SetUsingItem(int itemNum)
    {
        UsingItemUI.SetActive(true);
        UsingItemImage.sprite = ItemspriteList[itemNum];
        UsingItemSlider.value = 0;
    }

    public void UpdateUsingItem(float Value)
    {
        UsingItemSlider.value = Value;
    }

    public void UpdatePowerSprite(int num)
    {
        PowerSliderBackGround.sprite = PowerSpriteList[num];
    }
}
