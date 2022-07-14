# viagogo_coding_challenge


## Task 1

1. What should be your approach to getting the list of events?
    - Select all unique events in the event list 
    ```csharp
         var allEvents = events.Distinct();
    ```
2. How would you call the AddToEmail method in order to send the events in an email?
    - Iterate through each event and select
     ```csharp
         var allEvents = events.Distinct();

            foreach (var item in allEvents)
            {
                var customersInCity = Customers.Where(x => x.City == item.City);
               
                foreach (var cust in customersInCity)
                {
                    AddToEmail(cust, item);
                }
                
            }
    ```
3. What is the expected output if we only have the client John Smith?
 <p align="center">
<img src="/images/john_smith_output_1.png" 
        alt="Picture" 
        width="300" 
        height="100" 
        style="display: block; margin: 0 auto" />
  </p>
  
4. Do you believe there is a way to improve the code you first wrote?
   - AddToEmail can be changed to recive a list of customers as this will prevent the inner loop when calling AddToEmail
    ```csharp
         void AddToEmail(List<Customer> customers, Event e, int? price = null)
    ```

## Task 2

1. What should be your approach to getting the distance between the customer’s city and the other cities on the list?
    - claculate all the distance of each event from the city of the customer 
    ```csharp
         var allEventsDistance  = events.Distinct();
    ```
2. How would you call the AddToEmail method in order to send the events in an email?
    - Iterate through each customer
    - get the closes 5 events from customer City
     ```csharp
     
           foreach (var cust in Customers)
            {
                var eventDistance = GetEventsDistanceFromCity(cust.City, allEvents);
                foreach (var evt in eventDistance)
                    AddToEmail(cust, evt);
            }
     
       //helper method to get the closes 5 events
          static IEnumerable<Event> GetEventsDistanceFromCity(string fromCity, IEnumerable<Event> allEvents)
        {
            foreach (var e in allEvents)
            {
                e.Distance = GetDistance(fromCity, e.City);
            }
            return allEvents.OrderBy(x => x.Distance).Take(5);

        }
    ```
    
    - get the  events from customer birthday
     ```csharp
             foreach (var cust in Customers)
            {
                var eventDistance = GetEventsCloseToBirthDate(cust.Birthday, allEvents);
                foreach (var evt in eventDistance)
                    AddToEmail(cust, evt);
            }
          //helper method to get the customer Birthday 
        static IEnumerable<Event> GetEventsCloseToBirthDate(DateTime birthday, IEnumerable<Event> allEvents)
        {
            //select events same month as birtday
            var events = allEvents.Where(x => x.Date.Month == birthday.Month);
            return events;
        }
    ```
3. What is the expected output if we only have the client John Smith?
 <p align="center">
<img src="/images/john_smith_output_2.png" 
        alt="Picture" 
        width="300" 
        height="100" 
        style="display: block; margin: 0 auto" />
  </p>
  
4. Do you believe there is a way to improve the code you first wrote?
    - Events Class can be extended to have a distance from location 
    
    ```csharp
         public class Event
        {
            public string Name { get; set; }
            public string City { get; set; }
            public int Distance { get; set; }
        }
    ```
    
   - We can the order each Events based on Distance 
   - Use dynamic type to make it simple and shorter and cleaner(eliminates creating a distance vairable in event class)
    
    
    ```csharp
      static IEnumerable<Event> GetEventsDistanceFromCity(string fromCity, IEnumerable<Event> allEvents)
        {
            var events = new List<Event>();
            var eventWithDistances = allEvents.Select(x => new { distance = GetDistance(fromCity, x.City), x.City, x.Name});
            events.AddRange(eventWithDistances.OrderBy(x => x.distance).Take(5).Select(x => new Event { City = x.City, Name = x.Name }));
            return events;
        }
    ```
    
   - we can make the number of events to return dynamic and return X amount of event
      
       ```csharp
        static IEnumerable<Event> GetEventsDistanceFromCity(string fromCity, IEnumerable<Event> allEvents, int count = 5)
        {
            var events = new List<Event>();
            var eventWithDistances = allEvents.Select(x => new { distance = GetDistance(fromCity, x.City), x.City, x.Name});
            events.AddRange(eventWithDistances.OrderBy(x => x.distance).Take(count).Select(x => new Event { City = x.City, Name = x.Name }));
            return events;
        }
        ```

# Task 3
1.  If the GetDistance method is an API call which could fail or is too expensive, how will u
improve the code written in 2? Write the code.

    GetDistance should be converted to a asynchronus method  so that it can be awaited to free resources
    
    ```csharp
      static Task<int> GetDistanceAsync(string fromCity, string toCity)
        {
           //a long running task
           Task.Delay(5000);
            return Task.Factory.StartNew(() => AlphebiticalDistance(fromCity, toCity));
        }
    ```
 # Task 4 
1. If the GetDistance method can fail, we don't want the process to fail. What can be done? Code it. (Ask clarifying questions to be clear about what is expected business-wise)  
    
    - we do not add to Email if there is an error 
    - because of long running issue check if customer city and event city is same and return 0 to avoid distance calculation
    - Add a try catch to prevent exection bubbling
    
    ```csharp
        static async Task<IEnumerable<Event>> GetEventsDistanceFromCityAsync(string fromCity, IEnumerable<Event> allEvents)
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
                        e.Distance = await  GetDistanceAsync(fromCity, e.City);
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
    ```
# Task 5
1. If we also want to sort the resulting events by other fields like price, etc. to determine which ones to send to the customer, how would you implement it? Code it.
    - An helper method to help sort dynamuically 
 ```csharp
  var sortvalues = new List<Expression<Func<Event, object>>>
        {
             x => x.City,
             x => x.Date,
        };
       var sorted =  Sort<Event>(events, sortvalues, true );
        
        Console.WriteLine("\n\n\nSorting By City and then Date");
        foreach(var e in sorted)
        {
            Console.Out.WriteLine($"Event:  {e.Name} in {e.City} on {e.Date.ToLongDateString()}");
        }
        
         var sortByPrice = new List<Expression<Func<Event, object>>>
            {
                 x => GetPrice(x),
            };
            var sortedByPrice = Sort(events, sortByPrice, true);

         
            Console.WriteLine("\n\n\nSorting By Price");
            foreach (var e in sortedByPrice)
            {
                Console.Out.WriteLine($"Event:  {e.Name} in {e.City} for price ${GetPrice(e)}");
            }
          
            var currentLocation = "New York";   
           
            var sortByDistance = new List<Expression<Func<Event, object>>>
            {
                 x => GetDistance(currentLocation, x.City),
            };

            var sortedByDistance = Sort(events, sortByDistance, true);

            Console.WriteLine($"\n\n\nSorting By Distance From {currentLocation}");
         
            foreach (var e in sortedByDistance)
            {
                Console.Out.WriteLine($"Event:  {e.Name} in {e.City}  and  distance : {GetDistance(currentLocation, e.City)}");
            }
        
        //helper Sort function
        static IList<T> Sort<T>(IList<T> list,List<Expression<Func<T, object>>> orderExpressions, bool isAscending = false) where T :class
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
 ```
  - Output
  <p align="center">
<img src="/images/john_smith_output_3.png" 
        alt="Picture" 
        width="600" 
        height="200" 
        style="display: block; margin: 0 auto" />
  </p>
# One of the questions is: how do you verify that what you’ve done is correct.
    - Write Unit Test for each function

```csharp
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
```
  - Output
  <p align="center">
<img src="/images/john_smith_output_4.png" 
        alt="Picture" 
        width="600" 
        height="300" 
        style="display: block; margin: 0 auto" />
  </p>
