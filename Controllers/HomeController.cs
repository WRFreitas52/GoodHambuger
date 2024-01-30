using GoodHamburger.Data;
using GoodHamburger.Models;
using GoodHamburger.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GoodHamburger.Controllers
{
    public class HomeController : Controller
    {

        private readonly GoodHamburgerContext _dbContext;
        private readonly OrderService _orderService;

        public HomeController(GoodHamburgerContext dbContext, OrderService orderService)
        {
            _dbContext = dbContext;
            _orderService = orderService;

        }

        [HttpGet("sandwichesandextras")]
        public IActionResult ListSandwichesAndExtras()
        {

            var sandwichesAndExtras = new
            {
                Sandwiches = _dbContext.Sandwiches.ToList(),
                Extras = _dbContext.Extras.ToList()
            };

            return Ok(sandwichesAndExtras);
        }

        [HttpGet("sandwiches")]
        public IActionResult ListSandwiches()
        {
            var sandwiches = _dbContext.Sandwiches.ToList();
            return Ok(sandwiches);
        }

        [HttpGet("extras")]
        public IActionResult ListExtras()
        {
            var extras = _dbContext.Extras.ToList();
            return Ok(extras);
        }

        [HttpPost("order")]
        public IActionResult PlaceOrder([FromBody] List<string> itemNames)
        {
            return _orderService.PlaceOrder(itemNames);
        }

        [HttpGet("orders")]
        public IActionResult ListOrders()
        {
            var orders = _dbContext.Orders.ToList();
            return Ok(orders);
        }

        [HttpPut("order/{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] List<string> itemNames)
        {
            return _orderService.UpdateOrder(id, itemNames);
        }

        public IActionResult SendOrder(List<string> itemNames)
        {
            try
            {
                var totalAmount = _orderService.PlaceOrder(itemNames);
                return Ok(totalAmount);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error view or redirect to an error page
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult RemoveOrder(int id)
        {
            try
            {
                _orderService.RemoveOrder(id);
                return Ok("RemoveOrder");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error view or redirect to an error page
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }
    }
}
    
