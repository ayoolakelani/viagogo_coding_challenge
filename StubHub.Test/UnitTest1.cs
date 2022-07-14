using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Viagogo;

namespace StubHub.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDistance_From_Same_City_Should_Be_Zero()
        {
           var distance = Solution.GetDistance("New York", "New York");
            Assert.AreEqual(distance, 0);
        }
        [TestMethod]
        public void GetDistance_From_Different_City_Should_Be_X()
        {
            var distance = Solution.GetDistance("A", "B");
            Assert.AreEqual(distance, 1);
        }
        [TestMethod]
        public async Task GetDistanceAsync_From_Same_City_Should_Be_ZeroAsync()
        {
            var distance = await Solution.GetDistanceAsync("New York", "New York");
            Assert.AreEqual(distance, 0);
        }
        [TestMethod]
        public async Task GetDistanceAsync_From_Different_City_Should_Be_XAsync()
        {
            var distance = await Solution.GetDistanceAsync("A", "B");
            Assert.AreEqual(distance, 1);
        }
        [TestMethod]
        public void GetDistance_From_Same_City_Should_Not_be_Zero()
        {
            var evt = new Event { City = "New York", Name = "LadyGaga" };
            var price = Solution.GetPrice(evt);
            Assert.AreEqual(price, 151);
        }
        [TestMethod]
        public void When_Events_Less_Than_5_GetEventDistanceFromCity_Should_Return_5_or_Less()
        {
            var events = new List<Event>
                                        {
                                            new Event{ Name = "Phantom of the Opera", City = "New York", Date = new DateTime(2022,10,1)},
                                            new Event{ Name = "Metallica", City = "Los Angeles", Date = new DateTime(2022,6,17)}
            };
            var top5events = Solution.GetEventsDistanceFromCity("New York", events);
            Assert.IsTrue(top5events.Count() <= 5, "The actualCount was not greater than five");
        }
        [TestMethod]
        public void When_Events_More_Than_5_GetEventDistanceFromCityAsync_Should_Return_5()
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
      
            var top5events = Solution.GetEventsDistanceFromCity("New York", events);
            Assert.IsTrue(top5events.Count() == 5, "The actualCount must be 5");
        }
        public async Task When_Events_Less_Than_5_GetEventDistanceFromCityAsync_Should_Return_5_or_LessAsync()
        {
            var events = new List<Event>
                                        {
                                            new Event{ Name = "Phantom of the Opera", City = "New York", Date = new DateTime(2022,10,1)},
                                            new Event{ Name = "Metallica", City = "Los Angeles", Date = new DateTime(2022,6,17)}
            };
            var top5events = await Solution.GetEventsDistanceFromCityAsync("New York", events);
            Assert.IsTrue(top5events.Count() <= 5, "The actualCount was not greater than five");
        }
        [TestMethod]
        public async Task When_Events_More_Than_5_GetEventDistanceFromCity_Should_Return_5Async()
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

            var top5events = await Solution.GetEventsDistanceFromCityAsync("New York", events);
            Assert.IsTrue(top5events.Count() == 5, "The actualCount must be 5");
        }
        [TestMethod]
        public void GetEventsCloseToBirthDate_Should_Return_Event_In_Same_Month_As_Date()
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

            var dateevents = Solution.GetEventsCloseToBirthDate(new DateTime(1882,8,18), events);
            Assert.IsTrue(dateevents.All(x => x.Date.Month == 8), "All Months musr be 8");
        }
        [TestMethod]
        public void Sort_should_return_events_sorted_by_date()
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
            var sortvalues = new List<Expression<Func<Event, object>>>
            {
                 x => x.Date,
            };

            var sortedByDate = Solution.Sort(events, sortvalues, true);

             sortvalues = new List<Expression<Func<Event, object>>>
            {
                 x => x.City,
            };
            var sortedByCityDescending = Solution.Sort(events, sortvalues, false);

            Assert.IsTrue(sortedByDate.Count == events.Count, "list must equal same count");
            Assert.IsTrue(sortedByCityDescending.Count == events.Count, "list must equal same count");

            Assert.IsTrue(sortedByDate.First().Name == "Metallica" && sortedByDate.First().City == "Los Angeles");
            Assert.IsTrue(sortedByDate.ToArray()[1].Name == "LadyGaGa" && sortedByDate.ToArray()[1].City == "Boston");
         

            Assert.IsTrue(sortedByCityDescending.First().City == "Washington");
            Assert.IsTrue(sortedByDate.Last().City == "Boston");
        }



    }
}
