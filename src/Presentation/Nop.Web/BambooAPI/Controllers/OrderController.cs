using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;

namespace Nop.Web.BambooAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    #region Fields
    protected readonly IOrderService _orderService;
    protected readonly ICustomerService _customerService;
    #endregion

    #region Ctor
    public OrderController(
        IOrderService orderService,
        ICustomerService customerService)
    {
        _orderService = orderService;
        _customerService = customerService;
    }
    #endregion

    #region Endpoints

    [Authorize]
    [HttpGet("GetOrderDetailsByEmail")]
    public async Task<IActionResult> GetOrderDetailsByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return NotFound("Email cannot be empty");

        var customer = await _customerService.GetCustomerByEmailAsync(email);
        if (customer == null)
            return NotFound("Customer not found");

        var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
        if (!orders.Any())
            return NotFound("No orders found.");

        var orderList = orders.Select(order => new
        {
            OrderId = order.Id,
            TotalAmount = order.OrderTotal,
            OrderDate = order.CreatedOnUtc.ToString("yyyy-MM-dd")
        }).ToList();

        return Ok(orderList);
    }
    #endregion
}
