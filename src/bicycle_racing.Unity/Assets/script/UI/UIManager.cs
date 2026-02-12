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
            GoalUI.transform.position = new Vector3(GoalUI.transform.position.x + 8f,GoalUI.transform.position.y,GoalUI.transform.position.z);
        }
    }

    /// <summary>
    /// パワーゲージの初期化
    /// </summary>
    /// <param name="Max">パワーゲージの最大値</param>
    public void InitPowerSlider(float Max)
    {
        powerSlider.maxValue = Max;
    }

    /// <summary>
    /// スピードゲージの初期化
    /// </summary>
    /// <param name="Max">スピードゲージの最大値</param>
    public void InitSpeedSlider(float Max)
    {
        speedSlider.maxValue = Max;
    }

    /// <summary>
    /// パワーゲージの更新
    /// </summary>
    /// <param name="value">パワーゲージの値</param>
    public void UpdatePowerSlider(float value)
    {
        powerSlider.value = value;
    }

    /// <summary>
    /// スピードゲージの更新
    /// </summary>
    /// <param name="value">スピードゲージの値</param>
    public void UpdateSpeedSlider(float value)
    {
        speedSlider.value = value;
    }

    /// <summary>
    /// ラップ数の更新
    /// </summary>
    /// <param name="rap">ラップ数</param>
    public void UpdateRapTex(int rap)
    {
        RapTex.text = $"{rap}/3";
    }

    /// <summary>
    /// 自転車アニメーションのスピード変更
    /// </summary>
    /// <param name="speed">速さ</param>
    public void SetBikeAnimSpeed(int speed)
    {
        bikeAnimController.speed = speed;
    }

    /// <summary>
    /// ランキングUI設定
    /// </summary>
    /// <param name="rnk">自身のランキング</param>
    public void SetRankText(int rnk)
    {
        RankTex.text = $"{rnk}位";
    }

    /// <summary>
    /// 所持アイテムのUI変更
    /// </summary>
    /// <param name="itemNum">アイテムのインデックス</param>
    public void ChengeItemSprite(int itemNum)
    {
        ItemButton.sprite = ItemspriteList[itemNum];

    }

    /// <summary>
    /// 使用中アイテムのUI初期化
    /// </summary>
    /// <param name="itemNum">アイテムのインデックス</param>
    public void SetUsingItem(int itemNum)
    {
        UsingItemUI.SetActive(true);
        UsingItemImage.sprite = ItemspriteList[itemNum];
        UsingItemSlider.value = 0;
    }

    /// <summary>
    /// 使用中アイテムのUI更新
    /// </summary>
    /// <param name="Value">アイテムの残り使用時間</param>
    public void UpdateUsingItem(float Value)
    {
        UsingItemSlider.value = Value;
    }

    /// <summary>
    /// パワーゲージのUI更新
    /// </summary>
    /// <param name="num">パワーゲージのUIインデックス</param>
    public void UpdatePowerSprite(int num)
    {
        PowerSliderBackGround.sprite = PowerSpriteList[num];
    }
}
