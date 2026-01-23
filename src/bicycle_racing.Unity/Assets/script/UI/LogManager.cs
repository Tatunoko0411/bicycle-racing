using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(-50)]


public class LogManager : MonoBehaviour
{
    static Text text;
    [SerializeField]public static Canvas canvas;
   static float LogTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        text = GetComponentInChildren<Text>();
    }

    void Start()
    {



    }

    // Update is called once per frame
   void Update()
   {
        if (LogTime > 0)
        {
            LogTime -= Time.deltaTime;
            if (LogTime <= 0)
            {
                text.text = "";
            }
        }
   }

   public static void SetLogText(string log)
    {
        text.text = log;
        LogTime = 10f;
    }
}
