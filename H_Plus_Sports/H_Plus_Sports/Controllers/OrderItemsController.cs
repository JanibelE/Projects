using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H_Plus_Sports.Contracts;
using H_Plus_Sports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace H_Plus_Sports.Controllers
{
    [Produces("application/json")]
    [Route("api/OrderItems")]
    public class OrderItemsController : Controller
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemsController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        private async Task<bool> OrderItemExists(int id)
        {
            return await _orderItemRepository.Exists(id);
        }
        [HttpGet]
        [Produces(typeof(DbSet<OrderItem>))]
        public IActionResult GetOrderItem()
        {
            return new ObjectResult(_orderItemRepository.GetAll());
        }

        [HttpGet("{id}")]
        [Produces(typeof(DbSet<OrderItem>))]
        public async Task<IActionResult> GetOrderItem([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderItem = await _orderItemRepository.Find(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        [HttpPost]
        [Produces(typeof(OrderItem))]
        public async Task<IActionResult> PostOrderItem([FromBody]OrderItem orderItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _orderItemRepository.Add(orderItem);

            return CreatedAtAction("GetOrderItem", new { id = orderItem.OrderItemId }, orderItem);

        }

        [HttpPut("{id}")]
        [Produces(typeof(OrderItem))]
        public async Task<IActionResult> UpdateOrderItem([FromRoute]int id, [FromBody]OrderItem orderItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderItem.OrderItemId)
            {
                return BadRequest();
            }


            try
            {
                await _orderItemRepository.Update(orderItem);
                return Ok(orderItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Produces(typeof(OrderItem))]
        public async Task<IActionResult> DeleteOrderItem([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (!await OrderItemExists(id))
            {
                return NotFound();
            }

            await _orderItemRepository.Remove(id);

            return Ok();
        }


    }
}