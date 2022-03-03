using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FoodTruck
{
    public class Customer
    {
        public int CustomerNumber { get; set; }
        public DateTime ArrivalTime { get; set; }
        public Thread Thread { get; set; }
    }
}