using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab9_10CharpT
{
    public delegate void PriceChangeHandler(object sender, PriceEventArgs e);

    public class PriceEventArgs : EventArgs
    {
        public double NewPrice { get; }
        public string Trend { get; }
        public int Priority { get; } 

        public PriceEventArgs(double price, string trend, int priority = 2)
        {
            NewPrice = price;
            Trend = trend;
            Priority = priority;
        }
    }

    public class StockExchange
    {
        public event PriceChangeHandler PriceChanged;
        private Random rnd = new Random();
        private double currentPrice = 100.0;

        public int TotalEvents { get; private set; }
        public int BullActions { get; set; }
        public int BearActions { get; set; }

        public async Task RunMarketAsync(int rounds)
        {
            for (int i = 1; i <= rounds; i++)
            {
                double change = (rnd.NextDouble() * 10) - 5;
                currentPrice += change;
                string trend = change > 0 ? "up" : "down";
                int priority = rnd.Next(1, 3); 

                TotalEvents++;
                Console.WriteLine($"\n[Подія {i}] Ціна: {currentPrice:F2} (Пріоритет: {priority})");

                OnPriceChanged(new PriceEventArgs(currentPrice, trend, priority));

                await Task.Delay(800); 
            }
        }

        protected virtual void OnPriceChanged(PriceEventArgs e)
        {
            PriceChanged?.Invoke(this, e);
        }

        public void PrintStatistics()
        {
            Console.WriteLine("\n--- Статистика за період ---");
            Console.WriteLine($"Всього подій: {TotalEvents}");
            Console.WriteLine($"Активність Биків: {BullActions}");
            Console.WriteLine($"Активність Ведмедів: {BearActions}");
        }
    }

    public class Trader
    {
        public string Name { get; }
        protected StockExchange exchange;

        public Trader(string name, StockExchange ex)
        {
            Name = name;
            exchange = ex;
        }

        public virtual void React(object sender, PriceEventArgs e)
        {
            string prefix = e.Priority == 1 ? "[VIP Опрацювання]" : "[Стандарт]";

            if (e.Trend == "up" && this is Bull)
            {
                Console.WriteLine($"{prefix} Бик {Name} реагує на ріст.");
                exchange.BullActions++;
            }
            else if (e.Trend == "down" && this is Bear)
            {
                Console.WriteLine($"{prefix} Ведмідь {Name} реагує на падіння.");
                exchange.BearActions++;
            }
        }
    }

    public class Bull : Trader { public Bull(string name, StockExchange ex) : base(name, ex) { } }
    public class Bear : Trader { public Bear(string name, StockExchange ex) : base(name, ex) { } }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Запуск асинхронної біржі...");
            StockExchange exchange = new StockExchange();

            Bull bull = new Bull("Олександр", exchange);
            Bear bear = new Bear("Ігор", exchange);

            exchange.PriceChanged += bull.React;
            exchange.PriceChanged += bear.React;

            await exchange.RunMarketAsync(5);

            exchange.PrintStatistics();
        }
    }
}
