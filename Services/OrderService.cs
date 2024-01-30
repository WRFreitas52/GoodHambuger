using GoodHamburger.Data;
using GoodHamburger.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GoodHamburger.Services
{
    public class OrderService
    {
        private readonly GoodHamburgerContext _dbContext;

        public OrderService(GoodHamburgerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult PlaceOrder(List<string> itemNames)
        {
            // Validate if at least one sandwich is present in the order
            if (!itemNames.Any(item => _dbContext.Sandwiches.Any(s => s.Name.Equals(item, StringComparison.OrdinalIgnoreCase))))
            {
                return new BadRequestObjectResult("At least one sandwich is required in the order.");
            }

            // Validate if all items in the request are valid products
            var invalidItems = itemNames.Except(_dbContext.Sandwiches.Select(s => s.Name).Union(_dbContext.Extras.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)).ToList();
            if (invalidItems.Any())
            {
                return new BadRequestObjectResult($"Invalid items: {string.Join(", ", invalidItems)}");
            }

            var sandwich = _dbContext.Sandwiches.SingleOrDefault(s => itemNames.Contains(s.Name, StringComparer.OrdinalIgnoreCase));
            var extras = _dbContext.Extras.Where(e => itemNames.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();

            var order = new Order
            {
                Sandwich = sandwich,
                Extras = extras
            };

            try
            {
                order.TotalAmount = CalculateTotalAmount(order);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return new OkObjectResult(new { TotalAmount = order.TotalAmount });
        }
        private double CalculateTotalAmount(Order order)
        {
            // Check for duplicate items
            if (order.Sandwich != null && order.Extras.Any(e => e.Name.Equals("Fries", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Each order cannot contain both a sandwich and fries.");

            if (order.Sandwich != null && order.Extras.Any(e => e.Name.Equals("Soft drink", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Each order cannot contain both a sandwich and a soft drink.");

            // Apply discounts based on the selected items
            double discountPercentage = 0;

            if (order.Sandwich != null && order.Extras.Any(e => e.Name.Equals("Fries", StringComparison.OrdinalIgnoreCase) && e.IsDrink))
                discountPercentage = 0.2; // 20% discount
            else if (order.Sandwich != null && order.Extras.Any(e => e.Name.Equals("Soft drink", StringComparison.OrdinalIgnoreCase)))
                discountPercentage = 0.15; // 15% discount
            else if (order.Sandwich != null && order.Extras.Any(e => e.Name.Equals("Fries", StringComparison.OrdinalIgnoreCase)))
                discountPercentage = 0.1; // 10% discount

            // Calculate the total amount based on the original prices and applied discounts
            double totalAmount = 0;

            if (order.Sandwich != null)
            {
                // Apply discount if applicable
                totalAmount += order.Sandwich.Price * (1 - discountPercentage);
            }

            totalAmount += order.Extras.Sum(extra =>
            {
                // Apply discount if applicable
                return extra.Price * (1 - discountPercentage);
            });

            return totalAmount;
        }
        public IActionResult UpdateOrder(int id, List<string> itemNames)
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null)
            {
                return new NotFoundObjectResult($"Order with id {id} not found.");
            }

            // Validate if all items in the request are valid products
            var invalidItems = itemNames.Except(_dbContext.Sandwiches.Select(s => s.Name).Union(_dbContext.Extras.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)).ToList();
            if (invalidItems.Any())
            {
                return new BadRequestObjectResult($"Invalid items: {string.Join(", ", invalidItems)}");
            }

            order.Sandwich = _dbContext.Sandwiches.SingleOrDefault(s => itemNames.Contains(s.Name, StringComparer.OrdinalIgnoreCase));
            order.Extras = _dbContext.Extras.Where(e => itemNames.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            order.TotalAmount = CalculateTotalAmount(order);

            _dbContext.SaveChanges();

            return new OkObjectResult(new { Message = "Order updated successfully.", TotalAmount = order.TotalAmount });
        }

        public void RemoveOrder(int orderId)
        {
            var orderToRemove = _dbContext.Orders.Find(orderId);

            if (orderToRemove != null)
            {
                _dbContext.Orders.Remove(orderToRemove);
                _dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException($"Order with ID {orderId} not found.");
            }
        }
    }
}
