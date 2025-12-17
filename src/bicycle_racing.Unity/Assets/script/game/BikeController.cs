using rayzngames;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace rayzngames
{
    public class BikeController : MonoBehaviour
    {
        //TODO
        //・対戦履歴作る
        //・リザルト後の遷移類をちゃんとやる（順位表示とかも）

        BicycleVehicle bicycle;
        float DefaultMaxSteeringAngle;
        float DefaultLeanAngle;

        int rank;

        public bool controllingBike;

        float increaseSpeed = 50f;
        float decelerationSpeed = 0.12f;
        float maxSpeed = 800f;
        float BackSpeed = -1000.0f;
        public float speed;

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


        [SerializeField] Slider powerSlider;
        public GameManager gameManager;
        UIManager uiManager;

        Rigidbody rb;

        Quaternion turnRot;


        [SerializeField] NetWorkManager net;

        //そのチェックポイントにおける進行度
        public float progress => nowCheckPoint.GetProgress(transform.position);


        public float rogress = 0;
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
            rogress = progress;

            var p0 = transform.position;

            var v = nowCheckPoint.nextCheckPoint.transform.position - transform.position;
            v.y = 0;
            v = v.normalized * Time.deltaTime * speed;

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
                        speed = 0;
                        enabled = false;
                        if (controllingBike)
                        {
                            //uiManager.GoalUI.SetActive(true);
                            isGoal = true;
                            net.Goal(rank);
                        }
                        return;
                    }
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

            //スタート前からスピードを貯めれるようにしたいのでスピード関係を上に配置
            if (Input.GetKeyDown(KeyCode.Return) && downTime <= 0)
            {
                
                Riding();
            }

            if (downTime >= 0)
            {
                downTime -= Time.deltaTime;
                speed -= decelerationSpeed;
            }
           

            if (!isDrift && acceleTime <= 0)
            {
                speed -= decelerationSpeed;
            }
            else
            {
                acceleTime -= Time.deltaTime;
            }
            if (speed < 0)
            {
                speed = 0;
            }


            uiManager.UpdatePowerSlider(speed);
            uiManager.UpdateSpeedSlider(bicycle.currentSpeed);

            if (!gameManager.isStart) { return; }


            if (!isStartDash)
            {
                if(speed >= maxSpeed * 0.7f)
                {
                    
                    rb.linearVelocity = transform.forward * startDashPower;
                }

                isStartDash = true;
               
                nowCheckPoint = CheckPoint.StartPoint;
            }

            if ((!bicycle.braking && speed >= 0))
            {
                bicycle.verticalInput = speed;

                if(bicycle.currentSpeed <= SpeedLim * 0.6f && speed >= maxSpeed*0.7f)
                {
                    rb.linearVelocity = rb.linearVelocity + (transform.forward * 0.01f);
                }
            }
   
            bicycle.horizontalInput = Input.GetAxis("Horizontal");
            BrakingInput();

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
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.8f)
            {
                uiManager.SetBikeAnimSpeed(40);
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.6f)
            {
                uiManager.SetBikeAnimSpeed(50);
            }
            else if (bicycle.currentSpeed >= SpeedLim * 0.4f)
            {
                uiManager.SetBikeAnimSpeed(60);
            }
            else if (bicycle.currentSpeed < SpeedLim * 0.4f)
            {
                uiManager.SetBikeAnimSpeed(80);
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


        }



        //ブレーキ操作
        void BrakingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bicycle.braking = true;
                DriftTime = 0;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if((bicycle.horizontalInput >= 0.5 || bicycle.horizontalInput <= -0.5) &&
                    speed >= (maxSpeed * 0.6))
                {

                    bicycle.maxSteeringAngle = 60;
                    bicycle.maxLeanAngle = 40;
                    bicycle.braking = false;

                    if(bicycle.currentSteeringAngle >= 40 || bicycle.currentSteeringAngle <= -40)
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
                
                if (bicycle.currentSpeed <= 0.5f)
                {
                    bicycle.braking = false;
                    bicycle.verticalInput = BackSpeed;
                    Debug.Log(bicycle.verticalInput);

                    speed -= decelerationSpeed * 5;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
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

                        speed += increaseSpeed * 5;
                       
                    }
                    else if (DriftTime >= 3)
                    {
                        accelePower = 4;
                        acceleTime = 2;

                        speed += increaseSpeed * 3;

                    }
                    else if (DriftTime >= 1)
                    {
                        accelePower = 3;
                        acceleTime = 1.5f;

                       speed += increaseSpeed * 2;
                    }

                    rb.linearVelocity = rb.linearVelocity + (transform.forward * accelePower);

                    if(speed >= maxSpeed * 0.95f)
                    {
                        speed = maxSpeed * 0.95f;
                    }

                   isDrift = false;

                    bicycle.rearTrail.emitting = false;
                    if (bicycle.rearSmoke.isPlaying) { bicycle.rearSmoke.Stop(); }

                    bicycle.frontTrail.emitting = false;
                    if (bicycle.frontSmoke.isPlaying) { bicycle.frontSmoke.Stop(); }
                }
            }

        }

        //自転車の加速
        public void Riding()
        {
            if(bicycle.braking)
            {
                return;
            }

            if (speed <= maxSpeed * 0.5f)
            {
                speed += increaseSpeed * 1.3f;
            }
            else if (speed >= maxSpeed * 0.9f)
            {
                speed += increaseSpeed * 0.35f;
            }
            else
            {
                speed += increaseSpeed;
            }
            if (speed >= maxSpeed)
            {
                speed = maxSpeed;
                downTime = 3;
            }
            
        }

        //ドリフト操作
        public void Drift()
        {
            DriftTime += Time.deltaTime;
            bicycle.braking = false ;

            if (Input.GetAxis("Horizontal") >= 0.1f)
            {
                if (bicycle.currentSteeringAngle >= 40)
                {
                    rb.AddForce((transform.right) * 70000, ForceMode.Force);
                   
                }
                rb.AddForce((transform.right) * 20000, ForceMode.Force);
            }
            else if (Input.GetAxis("Horizontal") <= -0.1f)
            {
                if(bicycle.currentSteeringAngle <= -40)
                {
                    rb.AddForce((transform.right) * -70000, ForceMode.Force);
                    
                }
                rb.AddForce((transform.right) * -20000, ForceMode.Force);
                
            }

            rb.AddForce((transform.forward) * speed * 20, ForceMode.Force);
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
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            bicycle = GetComponent<BicycleVehicle>();
            speed = 0;


    
            rap = 1;

            DefaultMaxSteeringAngle = bicycle.maxSteeringAngle;
            DefaultLeanAngle = bicycle.maxLeanAngle;

            nowCheckPoint = CheckPoint.StartPoint;

            isStartDash = false;

            if(controllingBike)
            {
                uiManager.InitPowerSlider(maxSpeed);
                uiManager.InitSpeedSlider(DefaultSpeedLim + 5);
                net = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
            }

            gameManager.bikeControllers.Add(this);

            isGoal = false;


            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
         
        }

        private void OnTriggerEnter(Collider other)
        {
            
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
