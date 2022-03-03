using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCart
{
    public class FoodCart
    {
        public static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("CMD Line Args Error");
                return;
            }

            var numberWorkers = Convert.ToInt32(args[0]);
            var timeBetweenCustomers = Convert.ToDouble(args[1]);
            var averageServiceTime = Convert.ToInt32(args[2]);
            var maxSimRunTime = Convert.ToInt32(args[3]);

            var foodCart = new FoodieCart(numberWorkers, timeBetweenCustomers, averageServiceTime, maxSimRunTime);
            Console.WriteLine($"Workers: {numberWorkers}\n Mean Arrival Time: {timeBetweenCustomers}\n Mean Service Time: {averageServiceTime}\n Simulation Runtime: {maxSimRunTime}\n");
            foodCart.Run();
        }
    }
}