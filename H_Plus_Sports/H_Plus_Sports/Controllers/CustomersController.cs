using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using H_Plus_Sports.Contracts;
using H_Plus_Sports.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace H_Plus_Sports.Controllers
{
    [Produces("application/json")]
    [Route("api/Customers")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _customerRepository.Exist(id);
        }

        [HttpGet]
        [Produces(typeof(Customer))]
        public IActionResult GetCustomer()
        {
            var results = new ObjectResult(_customerRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            Request.HttpContext.Response.Headers.Add("X-Total-Count", _customerRepository.GetAll().Count().ToString());
            return results;
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> GetCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customer = await _customerRepository.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
            
            
        }

        [HttpPost]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _customerRepository.Add(customer);
            return CreatedAtAction("getCustomer", new { id = customer.CustomerId }, customer);
        }

        [HttpPut("{id}")]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> UpdateCustomer([FromRoute] int id, [FromBody] Customer customer)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id!= customer.CustomerId)
            {
                return BadRequest();
            }

            try
            {

                await _customerRepository.Update(customer);
                return Ok(customer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(id))
                {
                    return NotFound();
                }else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!await CustomerExists(id))
            {
                return NotFound();
            }

            await _customerRepository.Remove(id);
            return Ok();
        }

    }
}