using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H_Plus_Sports.Contracts;
using H_Plus_Sports.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSportsAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {

        private readonly IOrderRepository _orderRepository;
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        private async Task<bool> OrderExists(int id)
        {
            return await _orderRepository.Exist(id);
        }

        [HttpGet]
        [Produces(typeof(Order))]
        public IActionResult GetOrder()
        {
            return new ObjectResult(_orderRepository.GetAll());
        }

        [HttpGet("{id}", Name ="GetOrder")]
        [Produces(typeof(Order))]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var order = await _orderRepository.Find(id);

            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        [Produces(typeof(Order))]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _orderRepository.Add(order);
            }
            catch (Exception)
            {
                if (!await OrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            return CreatedAtAction("GetOrder", new { id = order.OrderId, order });

        }

        [HttpPut("{id}")]
        [Produces(typeof(Order))]
        public async Task<IActionResult> PutOrder([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            try
            {
                await _orderRepository.Update(order);
                return Ok(order);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
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
        [Produces(typeof(Order))]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(!await OrderExists(id))
            {
                return NotFound();
            }
            await _orderRepository.Remove(id);             
            return Ok();
        }
    }
}