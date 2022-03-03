using System.Collections.Concurrent;

namespace FoodCart
{
    public class FoodieCart
    {
        private static int _numberWorkers;
        private static double _timeBetweenCustomers;
        private static int _averageServiceTime;
        private static double _maxSimRunTime;

        private static int _customerCount;
        private static object _serviceEndTimeLock = new();

        private static int _maxLineSize = 10;

        private static Semaphore Workers;
        private static BlockingCollection<Thread> Line = new();


        private static DateTime _startTime = DateTime.Now;
        private static TimeSpan _timeSpan;
        private static TimeSpan _timeSpanTemp;
        private static TimeSpan _averageWaitingTimeNumber;
        private DateTime ServiceTimeEnd;

        public FoodieCart(int numberWorkers, double timeBetweenCustomers, int averageServiceTime, double maxSimRunTime)
        {
            _numberWorkers = numberWorkers;
            _timeBetweenCustomers = timeBetweenCustomers;
            _averageServiceTime = averageServiceTime;
            _maxSimRunTime = maxSimRunTime;
            Workers = new Semaphore(numberWorkers, numberWorkers);
        }

        public void QueueLine(object? obj)
        {
            DateTime runTime = DateTime.Now;
            DateTime endTime = runTime.AddSeconds(_maxSimRunTime);

            Random customerArrivalTime = new Random();
            DateTime current = DateTime.Now;
            DateTime addOne = current.AddSeconds(customerArrivalTime.Next((int)_timeBetweenCustomers, (int)(2 * _timeBetweenCustomers)));

            var i = 0;
            while (runTime != endTime)
            {
                if (current >= addOne && Line.Count < _maxLineSize)
                {
                    _timeSpan = runTime - _startTime;
                    _timeSpanTemp = _timeSpan;

                    var customer = new Thread(CustomerProcess)
                    {
                        Name = $"{++i}"
                    };

                    customer.Start();
                    Line.Add(customer);
                    _timeSpan = RoundSeconds(_timeSpan);
                    Console.WriteLine($"At time {_timeSpan.TotalSeconds} Customer {customer.Name} arrives in line\n");

                    Thread.Sleep(1000);

                    current = DateTime.Now;
                    addOne = current.AddSeconds(customerArrivalTime.Next((int)_timeBetweenCustomers, (int)(2 * _timeBetweenCustomers)));

                }

                current = current.AddSeconds(1);
                runTime = runTime.AddSeconds(1);
            }

        }

        public static TimeSpan RoundSeconds(TimeSpan span)
        {
            return TimeSpan.FromSeconds(Math.Round(span.TotalSeconds));
        }

        public void CustomerProcess(object? obj)
        {
            _customerCount++;

            Workers.WaitOne();
            Random serviceTime = new Random();

            DateTime start = DateTime.Now;
            DateTime end = start.AddSeconds(serviceTime.Next(_averageServiceTime, (int)(2 * _averageServiceTime)));

            TimeSpan timeSpan2 = (DateTime.Now - _startTime) + _timeSpanTemp;
            _averageWaitingTimeNumber += (timeSpan2 - _timeSpan);
            timeSpan2 = RoundSeconds(timeSpan2);
            Console.WriteLine($"At time {timeSpan2.TotalSeconds} Customer {Thread.CurrentThread.Name} starts being served\n");

            Thread.Sleep(serviceTime.Next(0, (int)(2 * _averageServiceTime)) * 2000);
            TimeSpan timeSpan3 = ((DateTime.Now - _startTime) + timeSpan2);
            timeSpan3 = RoundSeconds(timeSpan3);
            Console.WriteLine($"At time {timeSpan3.TotalSeconds} Customer {Thread.CurrentThread.Name} leaves the food cart\n");

            if (Line.Count == _maxLineSize)
            {
                Line.Take();
            }

            lock (_serviceEndTimeLock)
            {
                ServiceTimeEnd = DateTime.Now.AddSeconds(serviceTime.Next(_averageServiceTime, (int) (2 * _averageServiceTime)));
            }
            Workers.Release();
        }

        public void Run()
        {
            var lineThread = new Thread(QueueLine);
            lineThread.Start();

            var current = DateTime.Now;
            var end = current.AddSeconds(_maxSimRunTime);

            while (current < end)
            {
                current = DateTime.Now;
            }
            foreach (var customer in Line)
            {
                if (customer.IsAlive)
                {
                    customer.Join();
                }
            }

            Console.WriteLine($"Simulation terminated after {_customerCount} customers served");
            Console.WriteLine($"Average waiting time = {(_averageWaitingTimeNumber / _customerCount).TotalSeconds}");
        }

    }
}