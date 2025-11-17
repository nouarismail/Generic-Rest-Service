using System;
using System.Text.Json;
using OrdersApp.Models;

namespace OrdersApp.Data;

public static class DbInitializer
{
    public static void Seed(FormsDbContext db)
        {
            if (db.Entries.Any())
                return; 

            var now = DateTime.UtcNow;
            var rnd = new Random();
            var statuses = new[] { "Pending", "Shipped", "Delivered" };
            var priorities = new[] { "Low", "Medium", "High" };

            var entries = new List<FormEntry>();

            for (int i = 1; i <= 50; i++)
            {
                var status = statuses[rnd.Next(statuses.Length)];
                var priority = priorities[rnd.Next(priorities.Length)];

                var orderDate = now.AddDays(-rnd.Next(1, 60));
                DateTime? shipDate = status == "Pending"
                    ? null
                    : orderDate.AddDays(rnd.Next(1, 7));

                var anonymousOrder = new
                {
                    ID = i,
                    CustomerName = $"Customer {i}",
                    ShippingAddress = $"{i} Example Street, City {i}",
                    OrderStatus = status, 
                    OrderDate = orderDate,
                    ShipDate = shipDate,
                    FulfillmentPriority = priority, 
                    RequiresSignature = rnd.Next(0, 2) == 1
                };

                var json = JsonSerializer.Serialize(anonymousOrder);

                entries.Add(new FormEntry
                {
                    Id = Guid.NewGuid(),
                    FormType = "orders",
                    Data = json,
                    CreatedAt = now.AddMinutes(-i)
                });
            }

            db.Entries.AddRange(entries);
            db.SaveChanges();
        }
}
