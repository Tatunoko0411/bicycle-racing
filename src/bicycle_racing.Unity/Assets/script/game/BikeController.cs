using DG.Tweening;
using NUnit.Framework;
using rayzngames;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rayzngames
{
    public class BikeController : MonoBehaviour
    {


        public DynamicJoystick joystick;

        BicycleVehicle bicycle;
        float DefaultMaxSteeringAngle;
        float DefaultLeanAngle;

        int rank;

        public bool controllingBike;

        float increaseSpeed = 50f;
        float decelerationSpeed = 0.35f;
        float maxPower = 800f;
        float BackPower = -10000.0f;
        public float power;

        float downTime;

        public int rap;
        //今走っているチェックポイント
       public CheckPoint nowCheckPoint;

        //クリアしたチェックポイント数
        public int checkCount;

        public bool isGoal;

        float DefaultSpeedLim = 20;
        float SpeedLim;

        bool isDrift; 
        float DriftSeconds;
        float DriftPower;
        bool isAccele;
        float DriftTime;

        float accelePower;
        float acceleTime;

        bool isStartDash;
        float startDashPower = 10f;

        float SStime;
        float SlipTime;

        bool inSlope;

        bool isKeepPower;

        float ItemTime;

        [SerializeField] Slider powerSlider;
        public GameManager gameManager;
        UIManager uiManager;

        Rigidbody rb;

        Quaternion turnRot;

        bool isBraking;

        [SerializeField] NetWorkManager net;

        [SerializeField] GameObject speedLine;

        [SerializeField]public GameObject BananaPrefab;
        [SerializeField]public GameObject BedPrefab;

        //そのチェックポイントにおける進行度
        public float progress => nowCheckPoint.GetProgress(transform.position);


        public float rogress = 0;

        public float assistPower;

        Scale scale;

        public Item HaveItem;

        Item UsedItem;

        [SerializeField] public Text NameText;

        [SerializeField] GameObject CarObj;
        [SerializeField] GameObject ElectroObj;



       public enum Scale
        {
            Default = 0,
            Big,
            Small,
        }

        public enum Item
        {
            None = 0,
            Big,
            Small,
            PowerDown,
            SpeedDown,
            KeepPower,
            Max,
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {

            InitBike();
            
        }

        private void Start()
        {
          
        }
        // Update is called once per frame
        void Update()
        {
            //アイテム情報の更新

            if ((ItemTime > 0))
            {
                ItemTime -= Time.deltaTime;
                if (controllingBike)
                {
                    uiManager.UpdateUsingItem((5 - ItemTime) / 5);
                }
            }
            else
            {//仕様アイテムの初期化
                if (UsedItem != Item.None)
                {
                    ItemTime = 0;
                    
                    switch (UsedItem)
                    {

                        case Item.Big:
                            scale = Scale.Default;
                            SpeedLim = DefaultSpeedLim;
                            break;
                        case Item.Small:
                            scale = Scale.Default;
                            SpeedLim = DefaultSpeedLim;
                            break;

                        case Item.KeepPower:
                            isKeepPower = false;
                            ElectroObj.SetActive(false);
                            break;

                    }

                    UsedItem = Item.None;
                    if (controllingBike)
                    {
                        uiManager.UsingItemUI.SetActive(false);
                    }

                }
            }


            if (!controllingBike)
            {
                return;
            }

            ///
            /// 以下は自プレイヤーのみの処理
            ///


            //チェックポイントの判定用意
            Vector3 direction = Camera.main.transform.position - NameText.transform.position;
            NameText.transform.rotation = Quaternion.LookRotation(-direction);

            rogress = progress;

            var p0 = transform.position;
            p0.y = 0;

            var v = nowCheckPoint.nextCheckPoint.transform.position - transform.position;
            v.y = 0;
            v = v.normalized * Time.deltaTime * 80;

            //進行方向をすこし揺らす
            v = turnRot * v;
            var p1 = p0 + v;



            //チェックポイント通過が不安定なので次のチェックポイント通過判定もする
            if (nowCheckPoint.CheckIfPassed(p0, p1)||nowCheckPoint.nextCheckPoint.CheckIfPassed(p0, p1))
            {
                //チェックポイント通過
                SEManager.PlaySE(SEManager.SE.Rap);

                if (nowCheckPoint.nextCheckPoint.CheckIfPassed(p0, p1))
                {
                    nowCheckPoint = nowCheckPoint.nextCheckPoint.nextCheckPoint;
                    checkCount += 2;
                    net.PassCheck();
                }
                else
                {
                    nowCheckPoint = nowCheckPoint.nextCheckPoint;
                    checkCount++;
                }


                if (controllingBike)
                {
                    //全プレイヤーに通知
                    net.PassCheck();
                }

                if (nowCheckPoint == CheckPoint.StartPoint)
                {

                   // SEManager.PlaySE(SEManager.SE.Rap);
                    rap++;

                    if (rap > 3)
                    {
                      
                        power = 0;
                        enabled = false;
                        if (controllingBike)
                        {
                             uiManager.GoalUI.SetActive(true);
                            isGoal = true;
                            //全プレイヤーに通知
                            net.Goal(rank);

                            if(rank == 1)
                            {
                                net.UpdateUserCount(1, 1);
                            }
                            else
                            {
                                net.UpdateUserCount(1, 0);
                            }


                            //リザルトに遷移
                            StartCoroutine(gameManager.MoveResult());

                            net.GoalPlayerList[net.roomModel.ConnectionId] = net.myself;
                            SEManager.PlaySE(SEManager.SE.Goal);
                        }
                        return;
                    }
                   
                    uiManager.UpdateRapTex(rap);
            
                }


            }

       

            //スタート前からパワーを貯めれるようにしたいのでパワー関係を上に配置
            if (Input.GetKeyDown(KeyCode.Return) && downTime <= 0)
            {
                
                Riding();
            }

            if (downTime >= 0)
            {
                downTime -= Time.deltaTime;
                power -= decelerationSpeed*1.5f;
            }
           

            if (!isDrift && acceleTime <= 0)
            {
                power -= decelerationSpeed;
            }
            else
            {
                acceleTime -= Time.deltaTime;
            }
            if (power < 0)
            {
                power = 0;
            }

            //  パワーゲージのUI更新
            if (downTime > 0)
            {
                uiManager.UpdatePowerSprite(0);
            }
            else
            {
                if (power > maxPower * 0.8f)
                {
                    uiManager.UpdatePowerSprite(3);
                }
                else if (power > maxPower * 0.6f && power <= maxPower * 0.8f)
                {
                    uiManager.UpdatePowerSprite(2);
                }
                else if (power <= maxPower * 0.6f)
                {
                    uiManager.UpdatePowerSprite(1);
                }
            }


            uiManager.UpdatePowerSlider(power);
            uiManager.UpdateSpeedSlider(bicycle.currentSpeed);

            if (!gameManager.isStart)
            {//スタート前の移動制限
                rb.linearVelocity = new Vector3(0,rb.linearVelocity.y,0);
                rb.angularVelocity = Vector3.zero;
                return; 
            }


            if (!isStartDash)
            {//スタートダッシュ
                if(power >= maxPower * 0.7f)
                {
                    
                    rb.linearVelocity = transform.forward * startDashPower;
                }

                isStartDash = true;
               
                nowCheckPoint = CheckPoint.StartPoint;
            }

            if ((!bicycle.braking && power >= 0))
            {
                if (SlipTime <= 0)
                {//パワーをもとにスピード加算
                    bicycle.verticalInput = power;

                    if (bicycle.currentSpeed <= SpeedLim * 0.6f && power >= maxPower * 0.7f)
                    {
                        rb.linearVelocity = rb.linearVelocity + (transform.forward * 0.01f);
                    }
                }
                else
                {
                    SlipTime -= Time.deltaTime;
                }
            }
   

            ///
            /// PC操作関係
            ///
            bicycle.horizontalInput = joystick.Horizontal;

            if(Input.GetAxis("Horizontal") >= 0.1f|| Input.GetAxis("Horizontal") <= -0.1f)
            {
                bicycle.horizontalInput = Input.GetAxis("Horizontal");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                BrakingInput();
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                OffBrakingInput();
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                UseItem();
            }


            //ドリフト操作
            if (isDrift)
            {
                Drift();
            }

            //Extending functionality 
            bicycle.InControl(controllingBike);

            if (controllingBike)
            {
                //Constrains the Z rotation of the bike, when onground, and releases it when airborne.
                bicycle.ConstrainRotation(bicycle.OnGround());
            }
            else
            {
                bicycle.ConstrainRotation(false);
            }

            if (acceleTime >= 0)
            {//加速
                rb.AddForce(transform.forward * accelePower, ForceMode.Force);
                acceleTime -= Time.deltaTime;
            }

            //速度ごとのUI変更

            if (bicycle.currentSpeed >= SpeedLim)
            {//速度制限

                Debug.Log("早すぎ");
                rb.AddForce(transform.forward * (-30000f), ForceMode.Force);

                uiManager.SetBikeAnimSpeed(20);
                speedLine.SetActive(true);
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.8f)
            {
                uiManager.SetBikeAnimSpeed(40);
                speedLine.SetActive(true);
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.6f)
            {
                uiManager.SetBikeAnimSpeed(50);
                speedLine.SetActive(false);
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.4f)
            {
                uiManager.SetBikeAnimSpeed(60);
                speedLine.SetActive(false);
            }
            else if (bicycle.currentSpeed < SpeedLim * 0.4f)
            {
                uiManager.SetBikeAnimSpeed(80);
                speedLine.SetActive(false);
            }


            //飛び上がり対策
            if (rb.linearVelocity.y >= 0.5f && !inSlope)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z);
            }


            //SS動作
            if (SStime >= 0)
            {
                rb.linearVelocity = rb.linearVelocity + (transform.forward * 0.001f) ;

                SStime -= Time.deltaTime;
            }
            else
            {
                SpeedLim = DefaultSpeedLim;
            }


            turnRot = Quaternion.AngleAxis(Random.Range(-10f, 10f), Vector3.up);

            //大きさ変更
            ChengeScale();

            //キープパワー使用時の処理
            if (isKeepPower)
            {
                if(power <= maxPower * 0.9f)
                {
                    power += increaseSpeed;

                    if (power >= maxPower * 0.9f)
                    {
                        power = maxPower * 0.9f;
                    }
                }
            }
            
            //ブレーキ操作
            if(isBraking)
            {
                Braking();
            }

        }



        /// <summary>
        /// ブレーキ操作（実行時）
        /// </summary>
        public void BrakingInput()
        {
       
                bicycle.braking = true;
            isBraking = true;
                DriftTime = 0;
            


        }

        /// <summary>
        /// ブレーキ操作（入力時）
        /// </summary>
        public void Braking()
        {

            if (bicycle.braking)
            {
                if ((bicycle.horizontalInput >= 0.5 || bicycle.horizontalInput <= -0.5) &&
                    power >= (maxPower * 0.6))
                {

                    bicycle.maxSteeringAngle = 60;
                    bicycle.maxLeanAngle = 50;
                   

                    if (bicycle.currentSteeringAngle >= 40 || bicycle.currentSteeringAngle <= -40)
                    {//ドリフト操作に切り替え
                        bicycle.turnSmoothing = 0;
                        bicycle.leanSmoothing = 0;
                        isDrift = true;
                        bicycle.braking = false;

                        //ドリフト時の演出追加

                        bicycle.rearTrail.emitting = true;

                        if (!bicycle.rearSmoke.isPlaying) { bicycle.rearSmoke.Play(); }

                        bicycle.frontTrail.emitting = true;

                        if (!bicycle.frontSmoke.isPlaying) { bicycle.frontSmoke.Play(); }

                    }
                }


            }


            if (bicycle.currentSpeed <= 1f || power <= 10)
            {//バック走行
                bicycle.braking = false;
                bicycle.verticalInput = BackPower;
     

                rb.linearVelocity = -(transform.forward * 1.5f);

                power -= decelerationSpeed * 5;
            }
        }

        /// <summary>
        /// ブレーキ操作（終了時）
        /// </summary>
        public void OffBrakingInput()
        {
            isBraking = false;
                bicycle.braking = false;

                // bicycle.brakeForce = DefaultBrakeForce;
                bicycle.maxLeanAngle = DefaultLeanAngle;
                bicycle.maxSteeringAngle = DefaultMaxSteeringAngle;
                bicycle.turnSmoothing = 0.75f;
                bicycle.leanSmoothing = 0.3f;

                if (isDrift)
                {//ドリフト終了後の加速
                    accelePower = 0;

                    if (DriftTime >= 5)
                    {
                        accelePower = 8;
                        acceleTime = 3;

                        power += increaseSpeed * 5;

                    }
                    else if (DriftTime >= 3)
                    {
                        accelePower = 4;
                        acceleTime = 2;

                        power += increaseSpeed * 3;

                    }
                    else if (DriftTime >= 1)
                    {
                        accelePower = 3;
                        acceleTime = 1.5f;

                        power += increaseSpeed * 2;
                    }

                    rb.linearVelocity = rb.linearVelocity + (transform.forward * accelePower);

                    if (power >= maxPower * 0.95f)
                    {
                        power = maxPower * 0.95f;
                    }

                    isDrift = false;

                    //ドリフト時の演出解除

                    bicycle.rearTrail.emitting = false;
                    if (bicycle.rearSmoke.isPlaying) { bicycle.rearSmoke.Stop(); }

                    bicycle.frontTrail.emitting = false;
                    if (bicycle.frontSmoke.isPlaying) { bicycle.frontSmoke.Stop(); }
                }
            
        }

        /// <summary>
        /// 自転車の加速
        /// </summary>
        public void Riding()
        {
            if(bicycle.braking || downTime > 0)
            {
                return;
            }

            if (power <= maxPower * 0.5f)
            {
                power += increaseSpeed * 1.3f;
            }
            else if (power >= maxPower * 0.9f)
            {
                power += increaseSpeed * 0.45f;
            }
            else
            {
                power += increaseSpeed;
            }

            if (power >= maxPower)
            {
                power = maxPower;
                downTime = 4;
            }
            
        }

        /// <summary>
        /// ドリフト操作
        /// </summary>
        public void Drift()
        {
            DriftTime += Time.deltaTime;
            bicycle.braking = false ;

            //内側の移動が弱く感じたので内側の移動を強くしてます

            if (Input.GetAxis("Horizontal") >= 0.1f)
            {//右方向操作
                if (bicycle.currentSteeringAngle >= 40)
                {
                    rb.AddForce((transform.right) * 150000, ForceMode.Force);
                    transform.Rotate(new Vector3(0, 0.7f, 0));
                }
                rb.AddForce((transform.right) * 60000, ForceMode.Force);
                rb.angularVelocity = rb.angularVelocity + (transform.right * 0.075f);

            }
            else if (bicycle.horizontalInput <= -0.1f)
            {//左方向操作
                if(bicycle.currentSteeringAngle <= -40)
                {
                    rb.AddForce((transform.right) * -150000, ForceMode.Force);
                    transform.Rotate(new Vector3(0, -0.7f, 0));
                }
                rb.AddForce((transform.right) * -60000, ForceMode.Force);
                rb.angularVelocity = rb.angularVelocity +(-transform.right*0.075f);
                
            }

            rb.AddForce((transform.forward) * power * 20, ForceMode.Force);
        }

        /// <summary>
        /// ランキング設定
        /// </summary>
        /// <param name="rank">プレイヤーのランキング</param>
        public void SetRank(int rank)
        {
            this.rank = rank;

            if (controllingBike)
            {
                uiManager.SetRankText(this.rank);
            }
        }

        /// <summary>
        /// 自転車の初期化
        /// </summary>
        public void InitBike()
        {
            rb = GetComponent<Rigidbody>();

            if (SceneManager.GetActiveScene().name == "MatcingScene"|| SceneManager.GetActiveScene().name == "ResultScene")
            {//ゲーム中以外は操作不可にする
                rb.isKinematic = true;
                rb.useGravity = false;
                enabled = false;
                return;
            }

          
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            bicycle = GetComponent<BicycleVehicle>();
            power = 0;


    
            rap = 1;

            DefaultMaxSteeringAngle = bicycle.maxSteeringAngle;
            DefaultLeanAngle = bicycle.maxLeanAngle;

            nowCheckPoint = CheckPoint.StartPoint;

            isStartDash = false;
            isKeepPower = false;

            if(controllingBike)
            {
                uiManager.InitPowerSlider(maxPower);
                uiManager.InitSpeedSlider(DefaultSpeedLim + 5);
              net = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

                net.bikeController = this;

                NameText.text = net.myself.Name;
            }

            gameManager.bikeControllers.Add(this);

            isGoal = false;
            isKeepPower = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            HaveItem = Item.None;
         
        }


        /// <summary>
        /// 大きさの変更
        /// </summary>
        public void ChengeScale()
        {
            switch (scale)
            {
                case Scale.Default:
                    if (transform.localScale.z > 0.75f)
                    {
                        transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    else if (transform.localScale.z < 0.75f)
                    {
                        transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    break;
                case Scale.Big:
                    if (transform.localScale.z < 1.5f)
                    {
                        transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    break;
                case Scale.Small:
                    if (transform.localScale.z > 0.5f)
                    {
                        transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    }
                   
                    break;
            }
          


        }

        /// <summary>
        /// パワーダウン
        /// </summary>
        public void PowerDown()
        {
            power -= maxPower * 0.3f;
        }

        /// <summary>
        /// スピードダウン
        /// </summary>
        public void SpeedDown()
        {
            rb.linearVelocity = rb.linearVelocity - (transform.forward * 2.5f);
            SlipTime = 5f;
        }

        /// <summary>
        /// アイテム使用
        /// </summary>
        public void UseItem()
        {
           
            switch (HaveItem)
            {
    
                case Item.Big:
                    ItemTime = 10;
                    scale = Scale.Big;
                    SpeedLim = 22;
                    SEManager.PlaySE(SEManager.SE.Big);
                    uiManager.SetUsingItem((int)Item.Big);
                    break;
                case Item.Small:
                    ItemTime = 10;
                    scale = Scale.Small;
                    SpeedLim = 25;
                    SEManager.PlaySE(SEManager.SE.Small);
                    uiManager.SetUsingItem((int)Item.Small);
                    break;
                case Item.PowerDown:
                    SEManager.PlaySE(SEManager.SE.put);
                    net.PutItem();
                    HaveItem = Item.None;
                    break;
                case Item.SpeedDown:
                    SEManager.PlaySE(SEManager.SE.put);
                    net.PutItem();
                    HaveItem = Item.None;
                    break;
                case Item.KeepPower:
                    ItemTime = 10;
                    isKeepPower = true;
                    uiManager.SetUsingItem((int)Item.KeepPower);
                    ElectroObj.SetActive(true);
                    SEManager.PlaySE(SEManager.SE.Erectlo);
                    net.PutItem();
                    break;
                    case Item.None:
                    return;

                    

                    
            }
            
            UsedItem = HaveItem;
            HaveItem = Item.None;
            uiManager.ChengeItemSprite((int)HaveItem);
        }

        /// <summary>
        /// アイテム使用（タイプ指定）
        /// </summary>
        /// <param name="Type">使用するアイテムの種類</param>
        public void UseItem(int Type)
        {
            switch (Type)
            {
            
               
                case (int)Item.KeepPower:
                    ItemTime = 10;
                    isKeepPower = true;
                    
                    ElectroObj.SetActive(true);
                    break;
                case (int)Item.None:
                    return;




            }

            UsedItem = (Item)Type;
        }
            private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "ItemBox")
            {

                Destroy(other.gameObject);
                if (controllingBike)
                {
                    if (HaveItem == Item.None && UsedItem == Item.None)
                    {
                        SEManager.PlaySE(SEManager.SE.Break);
                        Item rnd = (Item)Random.Range(1, (int)Item.Max);
                        HaveItem = rnd;

                        Debug.Log(HaveItem);


                        //所持アイテムの画像差し替え、インデックスは　HaveItem　で引き出す
                        uiManager.ChengeItemSprite((int)HaveItem);
                    }
                }
            }

            if(other.gameObject.tag == "Banana")
            {
                Destroy(other.gameObject);
                if (scale == Scale.Big)
                {
                    return;
                }

                SEManager.PlaySE(SEManager.SE.Damage);
                SpeedDown();
               
            }

            if (other.gameObject.tag == "Bed")
            {
                Destroy(other.gameObject);
                if (scale == Scale.Big)
                {
                    return;
                }

                SEManager.PlaySE(SEManager.SE.Damage);
                PowerDown();
           
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "SSZone")
            {
                SStime += Time.deltaTime;

                if (SStime >= 3)
                {
                    SpeedLim = 25;
                    SStime = 3;
                
                }
            }

            if(other.gameObject.tag == "Saka")
            {
                inSlope = true;

            }
  
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Saka")
            {
                inSlope = false;

            }
        }

    }
}
