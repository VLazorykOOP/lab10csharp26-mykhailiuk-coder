// Задача 1. Моделювання подій
/*
 * 10) Створіть проект "Бики та ведмеді", де об'єкти класу "Бики" відіграють на біржі
 * на підвищення, а "Ведмеді" - на зниження.
 * Досліджуйте можливість створення класу, у якім одні об'єкти цього класу
 * створюють події, а інші об'єкти цього ж класу обробляють ці події.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndBears
{
    public delegate void PriceChangeHandler(object sender, PriceEventArgs e);
    public class PriceEventArgs : EventArgs
    {
        public double NewPrice { get; }
        public string Trend { get; } 

        public PriceEventArgs(double price, string trend)
        {
            NewPrice = price;
            Trend = trend;
        }
    }

    public class StockExchange
    {
        public event PriceChangeHandler PriceChanged;
        private Random rnd = new Random();
        private double currentPrice = 100.0;

        public void SimulateTrading(int rounds)
        {
            for (int i = 1; i <= rounds; i++)
            {
                double change = (rnd.NextDouble() * 10) - 5; 
                currentPrice += change;
                string trend = change > 0 ? "up" : "down";

                Console.WriteLine($"--- Раунд {i}: Ціна становить {currentPrice:F2} ---");

                OnPriceChanged(new PriceEventArgs(currentPrice, trend));
                System.Threading.Thread.Sleep(500);
            }
        }

        protected virtual void OnPriceChanged(PriceEventArgs e)
        {
            PriceChanged?.Invoke(this, e);
        }
    }

    public abstract class Trader
    {
        public string Name { get; }
        public Trader(string name) => Name = name;
        public abstract void ReactToMarket(object sender, PriceEventArgs e);
    }

    public class Bull : Trader
    {
        public Bull(string name) : base(name) { }
        public override void ReactToMarket(object sender, PriceEventArgs e)
        {
            if (e.Trend == "up")
                Console.WriteLine($"[Бик {Name}]: Ціна росте ({e.NewPrice:F2})! Купую активи.");
        }
    }

    public class Bear : Trader
    {
        public Bear(string name) : base(name) { }
        public override void ReactToMarket(object sender, PriceEventArgs e)
        {
            if (e.Trend == "down")
                Console.WriteLine($"[Ведмідь {Name}]: Ціна падає ({e.NewPrice:F2})! Продаю активи.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            StockExchange exchange = new StockExchange();

            Bull bull1 = new Bull("Олександр");
            Bear bear1 = new Bear("Ігор");

            exchange.PriceChanged += bull1.ReactToMarket;
            exchange.PriceChanged += bear1.ReactToMarket;

            Console.WriteLine("Початок торгів на біржі...");
            exchange.SimulateTrading(5);

            Console.WriteLine("\nТорги завершено.");
        }
    }
}