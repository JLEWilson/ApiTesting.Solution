using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTesting.Models;
using System.Linq;

namespace CretaceousPark.AddControllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiVersion("1.0")]
  [ApiVersion("2.0")]
  [ApiController]
  public class AnimalsController : ControllerBase
  {
    private readonly ApiTestingContext _db;

    public AnimalsController(ApiTestingContext db)
    {
      _db = db;
    }

    //GET api/animals
    [MapToApiVersion("1.0")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> Get(string species, string gender, string name)
    {
      var query = _db.Animals.AsQueryable();

      if(species != null)
      {
        query = query.Where(entry => entry.Species == species);
      }

      if(gender != null)
      {
        query = query.Where(entry => entry.Gender == gender);
      }

      if(gender != null)
      {
        query = query.Where(entry => entry.Name == name);
      }
      return await query.ToListAsync();
    }

    //GET api/animals
    [MapToApiVersion("2.0")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> Get2(string species, string gender, string name)
    {
      var query = _db.Animals.AsQueryable();

      if(species != null)
      {
        query = query.Where(entry => entry.Species == species);
      }

      if(gender != null)
      {
        query = query.Where(entry => entry.Gender == gender);
      }

      if(gender != null)
      {
        query = query.Where(entry => entry.Name == name);
      }

      query = query.OrderBy(animal => animal.Name);

      return await query.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(int id)
    {
      var animal = await _db.Animals.FindAsync(id);

      if (animal == null)
      {
        return NotFound();
      }

      return animal;
    }

    [HttpPost]
    public async Task<ActionResult<Animal>> Post(Animal animal)
    {
      _db.Animals.Add(animal);
      await _db.SaveChangesAsync();

      return CreatedAtAction(nameof(GetAnimal), new { id = animal.AnimalId }, animal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put( int id, Animal animal)
    {
      if(id != animal.AnimalId)
      {
        return BadRequest();
      }

      _db.Entry(animal).State = EntityState.Modified;

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if(!AnimalExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
      var animal = await _db.Animals.FindAsync(id);
      if(animal == null)
      {
        return NotFound();
      }

      _db.Animals.Remove(animal);
      await _db.SaveChangesAsync();

      return NoContent();
    }


    private bool AnimalExists(int id)
    {
      return _db.Animals.Any(e => e.AnimalId == id);
    }
  }
}