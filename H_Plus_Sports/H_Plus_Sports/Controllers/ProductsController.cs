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
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        public ProductsController(IProductRepository orderItemRepository)
        {
            _productRepository = orderItemRepository;
        }

        private async Task<bool> ProductExists(string id)
        {
            return await _productRepository.Exist(id);
        }

        [HttpGet]
        [Produces(typeof(Product))]
        public IActionResult GetProduct()
        {
            return new ObjectResult(_productRepository.GetAll());
        }

        [HttpGet("{id}", Name ="GetProduct")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> GetProduct([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productRepository.Find(id);
            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);

        }

        [HttpPost]
        [Produces(typeof(Product))]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try            
            {
                await _productRepository.Add(product);
            }
            catch (DbUpdateException)
            {
                if (!await ProductExists(product.ProductId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> PutProduct([FromRoute] string id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != product.ProductId)
            {
                return BadRequest();
            }

            try
            {
                await _productRepository.Update(product);
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(id))
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
        [Produces(typeof(Product))]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await ProductExists(id))
            {
                return NotFound();
            }

            await _productRepository.Remove(id);
            return Ok();
        }
    }
}