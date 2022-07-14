

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Viagogo
{
    public class Event
    {
        public string Name { get; set; }
        public string City { get; set; }

        public DateTime Date { get; set; }

        public int Distance { get; set; }

    }
    public class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
    }
    public class Solution
    {
        static async Task Main(string[] args)
        {
            var events = new List<Event>
                                        {
                                            new Event{ Name = "Phantom of the Opera", City = "New York", Date = new DateTime(2022,10,1)},
                                            new Event{ Name = "Metallica", City = "Los Angeles", Date = new DateTime(2022,6,17)},
                                            new Event{ Name = "Metallica", City = "New York", Date = new DateTime(2022,8,10)},
                                            new Event{ Name = "Metallica", City = "Boston", Date = new DateTime(2022,11,15)},
                                            new Event{ Name = "LadyGaGa", City = "New York", Date = new DateTime(2022,8,1)},
                                            new Event{ Name = "LadyGaGa", City = "Boston", Date = new DateTime(2022,7,25)},
                                            new Event{ Name = "LadyGaGa", City = "Chicago", Date = new DateTime(2022,10,1)},
                                            new Event{ Name = "LadyGaGa", City = "San Francisco", Date = new DateTime(2022,11,3)},
                                            new Event{ Name = "LadyGaGa", City = "Washington", Date = new DateTime(2022,10,1)},
                                        };

            //1. find out all events that arein cities of customer
            // then add to email.
            var Customers = new List<Customer> { new Customer { Name = "John Smith", City = "New York", Birthday = new DateTime(1985, 10, 31) } };

            // 1. TASK
            var allEvents = events.Distinct();

            foreach (var item in allEvents)
            {
                var customersInCity = Customers.Where(x => x.City == item.City);

                foreach (var cust in customersInCity)
                {
                    AddToEmail(cust, item);
                }

            }
            /*
            * We want you to send an email to this customer with all events in their city
            * Just call AddToEmail(customer, event) for each event you think they should get
            */

           // 2.TASK

            Console.Out.WriteLine("\n\nEvents Close to Customers City\n\n\n");
            foreach (var cust in Customers)
            {
                var eventDistance = GetEventsDistanceFromCity(cust.City, allEvents);
                foreach (var evt in eventDistance)
                    AddToEmail(cust, evt);
            }


            Console.Out.WriteLine("\n\nEvents Close to Customers Birthday\n\n\n");
            foreach (var cust in Customers)
            {
                var eventDistance = GetEventsCloseToBirthDate(cust.Birthday, allEvents);
                foreach (var evt in eventDistance)
                    AddToEmail(cust, evt);
            }


           // 3.TASK
            Console.Out.WriteLine("\n\nEvents Close to Customers City With Expensive Call \n\n\n");

            foreach (var cust in Customers)
            {
                var eventDistance = await GetEventsDistanceFromCityAsync(cust.City, allEvents);
                foreach (var evt in eventDistance)
                    AddToEmail(cust, evt);
            }


            // 5. TASK
            Console.Out.WriteLine("\n\nDynamic Sorting \n\n\n");

            

            var sortvalues = new List<Expression<Func<Event, object>>>
            {
                 x => x.City,
                 x => x.Date,
               
            };
           var sorted =  Sort<Event>(events, sortvalues, true );

            foreach(var e in sorted)
            {
                Console.Out.WriteLine($"Event:  {e.Name} in {e.City} on {e.Date.ToLongDateString()}");
            }

        }

        public static async Task<IEnumerable<Event>> GetEventsDistanceFromCityAsync(string fromCity, IEnumerable<Event> allEvents)
        {
            List<Event> events = new List<Event>();
            foreach (var e in allEvents)
            {
                try
                {
                    if (e.City == fromCity)
                    {
                        e.Distance = 0;
                    }
                    else
                    {
                        e.Distance = await GetDistanceAsync(fromCity, e.City);
                    }

                    events.Add(e);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error getting Distance From {fromCity} to {e.City}");
                }
            }
            return events.OrderBy(x => x.Distance).Take(5);

        }

        public static IEnumerable<Event> GetEventsDistanceFromCity(string fromCity, IEnumerable<Event> allEvents)
        {
            foreach (var e in allEvents)
            {

                e.Distance = GetDistance(fromCity, e.City);
            }
            return allEvents.OrderBy(x => x.Distance).Take(5);

        }

        //helper method to get the closes 5 events
        public static IEnumerable<Event> GetEventsCloseToBirthDate(DateTime birthday, IEnumerable<Event> allEvents)
        {

            //select events same month as birtday
            var events = allEvents.Where(x => x.Date.Month == birthday.Month);

            return events;
        }


        public static Task<int> GetDistanceAsync(string fromCity, string toCity)
        {

            Task.Delay(5000);
            return Task.Factory.StartNew(() =>  AlphebiticalDistance(fromCity, toCity));
        }

        // You do not need to know how these methods work
        public static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        public static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        public static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }

        public static IList<T> Sort<T>(IList<T> list, List<Expression<Func<T, object>>> orderExpressions, bool isAscending = false) where T : class
        {
            var query = list.AsQueryable();
            IOrderedQueryable<T> queryWithOrder = null;

            if (isAscending)
            {
                queryWithOrder = query.OrderBy(orderExpressions[0]);
            }
            else
            {
                queryWithOrder = query.OrderByDescending(orderExpressions[0]);
            }

            if (orderExpressions.Count > 1)
            {
                for (int i = 1; i < orderExpressions.Count; i++)
                {
                    if (isAscending)
                    {
                        queryWithOrder = queryWithOrder.ThenBy(orderExpressions[i]);
                    }
                    else
                    {
                        queryWithOrder = queryWithOrder.ThenByDescending(orderExpressions[i]);
                    }
                }
            }


            return queryWithOrder.ToList();
        }
    }

                    
}
/*
var customers = new List<Customer>{
new Customer{ Name = "Nathan", City = "New York"},
new Customer{ Name = "Bob", City = "Boston"},
new Customer{ Name = "Cindy", City = "Chicago"},
new Customer{ Name = "Lisa", City = "Los Angeles"}
};
*/