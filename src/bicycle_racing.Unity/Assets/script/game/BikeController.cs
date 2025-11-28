using rayzngames;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace rayzngames
{
    public class BikeController : MonoBehaviour
    {
        BicycleVehicle bicycle;
        float DefaultMaxSteeringAngle;
        float DefaultLeanAngle;

        int rank;

        public bool controllingBike;

        float increaseSpeed = 50f;
        float decelerationSpeed = 0.15f;
        float maxSpeed = 800f;
        float BackSpeed = -100.0f;
        public float speed;

        float downTime;

        public int rap;
        //今走っているチェックポイント
       public CheckPoint nowCheckPoint;

        //クリアしたチェックポイント数
        int _checkCount;
        public int checkCount => _checkCount;


        float DefaultSpeedLim = 22;
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




        [SerializeField] Slider powerSlider;
        GameManager gameManager;
        UIManager uiManager;

        Rigidbody rb;

        Quaternion turnRot;

        //そのチェックポイントにおける進行度
        public float progress => nowCheckPoint.GetProgress(transform.position);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            rb = GetComponent<Rigidbody>(); 
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            bicycle = GetComponent<BicycleVehicle>();
            speed = 0;

            uiManager.InitPowerSlider(maxSpeed);
            rap = 1;

            DefaultMaxSteeringAngle = bicycle.maxSteeringAngle;
            DefaultLeanAngle = bicycle.maxLeanAngle;

            nowCheckPoint = CheckPoint.StartPoint;

            isStartDash = false;
        }
        // Update is called once per frame
        void Update()
        {
            //TODO:漕ぎすぎデバフ

            //スタート前からスピードを貯めれるようにしたいのでスピード関係を上に配置
            if (Input.GetKeyDown(KeyCode.Return) && downTime <= 0)
            {
                
                Riding();
            }

            if (downTime >= 0)
            {
                downTime -= Time.deltaTime;
                speed -= decelerationSpeed * 0.5f;
            }
           

            if (!isDrift && acceleTime <= 0)
            {
                speed -= decelerationSpeed;
            }
            if (speed < 0)
            {
                speed = 0;
            }


            uiManager.UpdatePowerSlider(speed);

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

            if(bicycle.currentSpeed >= SpeedLim)
            {

                Debug.Log("早すぎ");
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x - (rb.linearVelocity.x * 0.05f),
                    rb.linearVelocity.y,
                    rb.linearVelocity.z - (rb.linearVelocity.z * 0.05f)
                    );
            }

            if(rb.linearVelocity.y >= 0.5f)
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
                _checkCount++;

                if(nowCheckPoint == CheckPoint.StartPoint)
                {
                    rap++;

                    if (rap > 3)
                    {
                        Debug.Log("ゴール！");
                        speed = 0;
                        enabled = false;
                        uiManager.GoalUI.SetActive(true);
                        return;
                    }
                    uiManager.UpdateRapTex(rap);
                    Debug.Log("一周");
                }
                Debug.Log("通過");
                
            }

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
                
                if (speed <= 0)
                {
                    bicycle.braking = false;
                    bicycle.verticalInput = BackSpeed;
                    Debug.Log(bicycle.verticalInput);
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
                       
                    }
                    else if (DriftTime >= 3)
                    {
                        accelePower = 4;

                    }
                    else if (DriftTime >= 1)
                    {
                        accelePower = 3;
                    }

                    rb.linearVelocity = rb.linearVelocity + (transform.forward * accelePower);

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
                speed += increaseSpeed * 1.2f;
            }
            else if (speed >= maxSpeed * 0.8f)
            {
                speed += increaseSpeed * 0.3f;
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
                    rb.AddForce((transform.right) * 50000, ForceMode.Force);
                   
                }
                rb.AddForce((transform.right) * 20000, ForceMode.Force);
            }
            else if (Input.GetAxis("Horizontal") <= -0.1f)
            {
                if(bicycle.currentSteeringAngle <= -40)
                {
                    rb.AddForce((transform.right) * -50000, ForceMode.Force);
                    
                }
                rb.AddForce((transform.right) * -20000, ForceMode.Force);
                
            }

            rb.AddForce((transform.forward) * speed * 20, ForceMode.Force);
        }

        public void SetRank(int rank)
        {
            this.rank = rank + 1;


        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "goal")
            {
                //if (PassCheck)
                //{
                //    rap++;
                //    
                //    PassCheck = false;

                //    uiManager.UpdateRapTex(rap);
                //}
            }

            if(other.gameObject.tag == "checkPoint")
            {
                //PassCheck = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "SSZone")
            {
                SStime += Time.deltaTime;

                if (SStime >= 3)
                {
                    SpeedLim = 28;
                    SStime = 3;
                
                }
            }
        }

    }
}
