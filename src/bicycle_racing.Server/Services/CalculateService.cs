using MagicOnion;
using MagicOnion.Server;
using bicycle_racing.Shared.Interfaces;


namespace realtime_game.Server.Services
{
    public class CalclateService : ServiceBase<ICalculateService>, ICalculateService
    {
        // 『乗算API』二つの整数を引数で受け取り乗算値を返す
        public async UnaryResult<int> MulAsync(int x, int y)
        {
            Console.WriteLine("Received:" + x + "," + y);
            return x * y;
        }

        public async UnaryResult<int> SumAllAsync(int[] numList)
        {
            int sum = 0;
            foreach (int num in numList)
            {
                sum += num;
            }

            return sum;
        }

        public async UnaryResult<int[]> CalcForOperationAsync(int x, int y)
        {
            int[] operation = { x + y, x - y, x * y, x / y };
            return operation;
        }

        public async UnaryResult<float> SumAllNumberAsync(Number numData)
        {
            float sum = 0;
            sum += numData.x;
            sum += numData.y;
            sum += numData.z;

            return sum;
        }

    }
}
