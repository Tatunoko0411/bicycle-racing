using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BaseModel : MonoBehaviour
{
#if DEBUG
    //protected const string ServerURL = "http://localhost:5244";
    protected const string ServerURL = "http://ge202400.japaneast.cloudapp.azure.com:25558/";

#else
    protected const string ServerURL = "http://ge202400.japaneast.cloudapp.azure.com:25558/";
#endif

}

