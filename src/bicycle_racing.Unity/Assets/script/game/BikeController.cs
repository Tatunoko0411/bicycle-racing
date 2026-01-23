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
        //TODO
        //・対戦履歴作る
        //・リザルト後の遷移類をちゃんとやる（順位表示とかも）

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
        float DefaultBrakeForce = 150;
        float DriftBrakeForce = 75;
        float DriftTime;

        float accelePower;
        float acceleTime;

        bool isStartDash;
        float startDashPower = 10f;

        float SStime;

        bool inSlope;

        bool isKeepPower;

        bool isAssist;

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


       public enum Scale
        {
            Default = 0,
            Big,
            Small,
        }

        public enum Item
        {
            None = 0,
            Assist,
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
            Vector3 direction = Camera.main.transform.position - NameText.transform.position;
            NameText.transform.rotation = Quaternion.LookRotation(-direction);

            rogress = progress;

            var p0 = transform.position;

            var v = nowCheckPoint.nextCheckPoint.transform.position - transform.position;
            v.y = 0;
            v = v.normalized * Time.deltaTime * 500;

            //進行方向をすこし揺らす
            v = turnRot * v;
            var p1 = p0 + v;

            if (nowCheckPoint.CheckIfPassed(p0, p1))
            {
                //チェックポイント通過
                nowCheckPoint = nowCheckPoint.nextCheckPoint;
                checkCount++;

                if(controllingBike)
                {
                   net.PassCheck();
                }

                if (nowCheckPoint == CheckPoint.StartPoint)
                {
                    rap++;

                    if (rap > 3)
                    {
                        Debug.Log("ゴール！");
                        power = 0;
                        enabled = false;
                        if (controllingBike)
                        {
                            //uiManager.GoalUI.SetActive(true);
                            isGoal = true;
                            net.Goal(rank);

                            StartCoroutine(gameManager.MoveResult());
                            net.GoalPlayerList[net.roomModel.ConnectionId] = net.myself;
                            SEManager.PlaySE(SEManager.SE.Goal);
                        }
                        return;
                    }
                    SEManager.PlaySE(SEManager.SE.Rap);
                    uiManager.UpdateRapTex(rap);
                    Debug.Log("一周");
                }
                Debug.Log("通過");

            }

            if (!controllingBike)
            {
                return;
            }
            //TODO:漕ぎすぎデバフ

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
                else if (power > maxPower * 0.4f && power <= maxPower * 0.8f)
                {
                    uiManager.UpdatePowerSprite(2);
                }
                else if (power <= maxPower * 0.4f)
                {
                    uiManager.UpdatePowerSprite(1);
                }
            }


            uiManager.UpdatePowerSlider(power);
            uiManager.UpdateSpeedSlider(bicycle.currentSpeed);

            if (!gameManager.isStart)
            {
                rb.linearVelocity = new Vector3(0,rb.linearVelocity.y,0);
                rb.angularVelocity = Vector3.zero;
                return; 
            }


            if (!isStartDash)
            {
                if(power >= maxPower * 0.7f)
                {
                    
                    rb.linearVelocity = transform.forward * startDashPower;
                }

                isStartDash = true;
               
                nowCheckPoint = CheckPoint.StartPoint;
            }

            if ((!bicycle.braking && power >= 0))
            {
                bicycle.verticalInput = power;

                if(bicycle.currentSpeed <= SpeedLim * 0.6f && power >= maxPower*0.7f)
                {
                    rb.linearVelocity = rb.linearVelocity + (transform.forward * 0.01f);
                }
            }
   
            bicycle.horizontalInput = joystick.Horizontal;
         

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
            {
                rb.AddForce(transform.forward * accelePower, ForceMode.Force);
                acceleTime -= Time.deltaTime;
            }

            if (bicycle.currentSpeed >= SpeedLim)
            {

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



            if (rb.linearVelocity.y >= 0.5f && !inSlope)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z);
            }

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

            if(isAssist)
            {
                HandleAssist();
            }

           // HandleAssist();
            ChengeScale();

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

            if ((ItemTime > 0))
            {
                ItemTime -= Time.deltaTime;
                uiManager.UpdateUsingItem((5-ItemTime)/5);
            }
            else
            {
                if (UsedItem != Item.None)
                {
                    ItemTime = 0;

                    switch (UsedItem)
                    {
                        case Item.Assist:
                            isAssist = false;
                            break;
                        case Item.Big:
                            scale = Scale.Default;
                            break;
                        case Item.Small:
                            scale = Scale.Default;
                            break;
                  
                        case Item.KeepPower:
                            isKeepPower = false;
                            break;

                    }

                    UsedItem = Item.None;
                    uiManager.UsingItemUI.SetActive(false);

                }
            }
            
            if(isBraking)
            {
                Braking();
            }

        }



        //ブレーキ操作
        public void BrakingInput()
        {
       
                bicycle.braking = true;
            isBraking = true;
                DriftTime = 0;
            


        }

        public void Braking()
        {

            if (bicycle.braking)
            {
                if ((bicycle.horizontalInput >= 0.5 || bicycle.horizontalInput <= -0.5) &&
                    power >= (maxPower * 0.6))
                {

                    bicycle.maxSteeringAngle = 60;
                    bicycle.maxLeanAngle = 40;
                    bicycle.braking = false;

                    if (bicycle.currentSteeringAngle >= 40 || bicycle.currentSteeringAngle <= -40)
                    {
                        bicycle.turnSmoothing = 0;
                        bicycle.leanSmoothing = 0;
                        isDrift = true;

                        bicycle.rearTrail.emitting = true;

                        if (!bicycle.rearSmoke.isPlaying) { bicycle.rearSmoke.Play(); }

                        bicycle.frontTrail.emitting = true;

                        if (!bicycle.frontSmoke.isPlaying) { bicycle.frontSmoke.Play(); }

                    }
                }


            }


            if (bicycle.currentSpeed <= 1f || power <= 10)
            {
                bicycle.braking = false;
                bicycle.verticalInput = BackPower;
     

                rb.linearVelocity = -(transform.forward * 1.5f);

                power -= decelerationSpeed * 5;
            }
        }

        public void OffBrakingInput()
        {

            Debug.Log("ブレーキ終わり");
            isBraking = false;
                bicycle.braking = false;

                // bicycle.brakeForce = DefaultBrakeForce;
                bicycle.maxLeanAngle = DefaultLeanAngle;
                bicycle.maxSteeringAngle = DefaultMaxSteeringAngle;
                bicycle.turnSmoothing = 0.75f;
                bicycle.leanSmoothing = 0.3f;

                if (isDrift)
                {
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

                    bicycle.rearTrail.emitting = false;
                    if (bicycle.rearSmoke.isPlaying) { bicycle.rearSmoke.Stop(); }

                    bicycle.frontTrail.emitting = false;
                    if (bicycle.frontSmoke.isPlaying) { bicycle.frontSmoke.Stop(); }
                }
            
        }

        //自転車の加速
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
                power += increaseSpeed * 0.35f;
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

        //ドリフト操作
        public void Drift()
        {
            DriftTime += Time.deltaTime;
            bicycle.braking = false ;

            if (joystick.Horizontal >= 0.1f)
            {
                if (bicycle.currentSteeringAngle >= 40)
                {
                    rb.AddForce((transform.right) * 90000, ForceMode.Force);
                    transform.Rotate(new Vector3(0, 0.45f, 0));
                }
                rb.AddForce((transform.right) * 40000, ForceMode.Force);
                
               
            }
            else if (joystick.Horizontal <= -0.1f)
            {
                if(bicycle.currentSteeringAngle <= -40)
                {
                    rb.AddForce((transform.right) * -90000, ForceMode.Force);
                    transform.Rotate(new Vector3(0, -0.45f, 0));
                }
                rb.AddForce((transform.right) * -40000, ForceMode.Force);
                rb.angularVelocity = rb.angularVelocity +(-transform.right*0.075f);
                
            }

            rb.AddForce((transform.forward) * power * 20, ForceMode.Force);
        }

        public void SetRank(int rank)
        {
            this.rank = rank;

            if (controllingBike)
            {
                uiManager.SetRankText(this.rank);
            }
        }


        public void InitBike()
        {
            rb = GetComponent<Rigidbody>();

            if (SceneManager.GetActiveScene().name == "MatcingScene"|| SceneManager.GetActiveScene().name == "ResultScene")
            {
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
            isAssist = false;
            isKeepPower = false;

            if(controllingBike)
            {
                uiManager.InitPowerSlider(maxPower);
                uiManager.InitSpeedSlider(DefaultSpeedLim + 5);
              net = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
            }

            gameManager.bikeControllers.Add(this);

            isGoal = false;
            isKeepPower = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            HaveItem = Item.None;
         
        }

        void HandleAssist()
        {
           

            var p = (transform.position-nowCheckPoint.nextCheckPoint.transform.position).normalized;

            float dist = Vector3.Dot(p,nowCheckPoint.nextCheckPoint.forward);


            Vector3 targetPos = nowCheckPoint.nextCheckPoint.transform.position + p * dist;

            targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);

           this.transform.DOLookAt(targetPos, 1f);

            Vector3 movePos= Vector3.MoveTowards(
                transform.position,
                targetPos,
                assistPower*Time.deltaTime
                );

            movePos -= transform.forward * assistPower * Time.deltaTime;
            
            transform.position = movePos;
        }

        public void ChengeScale()
        {
            switch (scale)
            {
                case Scale.Default:
                    if (transform.localScale.x > 0.75f)
                    {
                        transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    else if (transform.localScale.x < 0.75f)
                    {
                        transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    break;
                case Scale.Big:
                    if (transform.localScale.x < 2f)
                    {
                        transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                    }
                    break;
                case Scale.Small:
                    if (transform.localScale.x > 0.3f)
                    {
                        transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    }
                   
                    break;
            }
          


        }

        public void  TestBigScale()
        {
            scale = Scale.Big;
        }

        public void TestSmallScale() { scale = Scale.Small; }
         
        public void TestDefaultScale() {  scale = Scale.Default; }

        public void ChengeScaleType(Scale scale)
        {
            this.scale = scale;
        }

        public void PowerDown()
        {
            power -= maxPower * 0.3f;
        }

        public void SpeedDown()
        {
            rb.linearVelocity = rb.linearVelocity - (transform.forward * 0.2f);
        }

       

        public void PutItem(Vector3 pos ,int ItemType)
        {
            Vector3 PutPos = new Vector3(pos.x, transform.position.y - 0.5f,transform.position.z);
            Instantiate(BananaPrefab,
               PutPos - (transform.forward * 2f),
                Quaternion.identity);

            SEManager.PlaySE(SEManager.SE.put);
        }

        public void UseItem()
        {
           
            switch (HaveItem)
            {
                case Item.Assist:
                    ItemTime = 10;
                    isAssist = true;
                    uiManager.SetUsingItem((int)Item.Assist);
                    break;
                case Item.Big:
                    ItemTime = 10;
                    scale = Scale.Big;
                    SEManager.PlaySE(SEManager.SE.Big);
                    uiManager.SetUsingItem((int)Item.Big);
                    break;
                case Item.Small:
                    ItemTime = 10;
                    scale = Scale.Small;
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
                    break;
                    case Item.None:
                    return;

                    

                    
            }
            
            UsedItem = HaveItem;
            HaveItem = Item.None;
            uiManager.ChengeItemSprite((int)HaveItem);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "ItemBox")
            {
                Destroy(other.gameObject);
                if (HaveItem == Item.None && UsedItem == Item.None)
                {
                    SEManager.PlaySE(SEManager.SE.Break);
                    Item rnd = (Item)Random.Range(1, (int)Item.Max - 1);
                    HaveItem = rnd;

                    Debug.Log(HaveItem);


                    //所持アイテムの画像差し替え、インデックスは　HaveItem　で引き出す
                    uiManager.ChengeItemSprite((int)HaveItem);
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
