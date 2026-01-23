using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using UnityEngine;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using bicycle_racing.Shared.Models;
using bicycle_racing.Shared.Interfaces;



public class CalculateModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:5244";
    async void Start()
    {
        //int result = await Mul(100, 323);
        //Debug.Log(result);
        //int[] numList = new int[3] {1,2,3 };
        //int all = await SumAll(numList);  
        //Debug.Log(all);

        //int[] ope = await CalcForOperation(4, 2);

        //foreach (int i in ope)
        //{
        //    Debug.Log(i);
        //}

        //Number number = new Number();
        //number.x = 1.2f;
        //number.y = 2.3f;
        //number.z = 3.1f;
        //float num = await SumAllNumber(number);
        //Debug.Log(num);
    }

    public async UniTask<int> Mul(int x, int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.MulAsync(x, y);
        return result;
    }

    public async UniTask<int> SumAll(int[] numList)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.SumAllAsync(numList);

        return result;
    }

    public async UnaryResult<int[]> CalcForOperation(int x, int y)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.CalcForOperationAsync(x,y);

        return result;
    }

    public async UnaryResult<float> SumAllNumber(Number numData)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<ICalculateService>(channel);
        var result = await client.SumAllNumberAsync(numData);

        return result;
    }

}
