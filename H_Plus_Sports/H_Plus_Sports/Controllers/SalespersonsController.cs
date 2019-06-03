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
    [Route("api/Salespersons")]
    public class SalespersonsController : Controller
    {
        private readonly ISalespersonRepository _salespersonRepository;
        public SalespersonsController(ISalespersonRepository salespersonRepository)
        {
            _salespersonRepository = salespersonRepository;
        }

        private async Task<bool> SalespersonExists(int id)
        {
            return await _salespersonRepository.Exists(id);
        }

        [HttpGet]
        [Produces(typeof(DbSet<Salesperson>))]
        public IActionResult GetSalesperson()
        {
            return new ObjectResult(_salespersonRepository.GetAll());
        }

        [HttpGet("{id}")]
        [Produces(typeof(DbSet<Salesperson>))]
        public async Task<IActionResult> GetSalesperson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesperson = await _salespersonRepository.Find(id);

            if (salesperson == null)
            {
                return NotFound();
            }

            return Ok(salesperson);
        }

        [HttpPost]
        [Produces(typeof(DbSet<Salesperson>))]
        public async Task<IActionResult> PostSalesperson([FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _salespersonRepository.Add(salesperson);

            return CreatedAtAction("GetSalesperson", new { id = salesperson.SalespersonId }, salesperson);
        }

        [HttpPut("{id}")]
        [Produces(typeof(DbSet<Salesperson>))]
        public async Task<IActionResult> PutSalesperson([FromRoute] int id, [FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesperson.SalespersonId)
            {
                return BadRequest();
            }


            try
            {
                await _salespersonRepository.Update(salesperson);
                return Ok(salesperson);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SalespersonExists(id))
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
        [Produces(typeof(DbSet<Salesperson>))]
        public async Task<IActionResult> DeleteSalesperson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!await SalespersonExists(id))
            {
                return NotFound();
            }

            await _salespersonRepository.Remove(id);

            return Ok();
        }
    }
}