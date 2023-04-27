using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreWebApi.Models;

namespace NetCoreWebApi.Controllers;

/// <summary>
/// Controller for entities of type <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class GenericController<T> : ControllerBase where T : class
{
    private readonly ILogger<GenericController<T>> _logger;
    private readonly DbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="context">Database context</param>
    public GenericController(ILogger<GenericController<T>> logger, NorthwindContext context)
    {
        this._logger = logger;
        _context = context;
    }

    /// <summary> Gets all T </summary>
    /// <returns/>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAllAsync()
    {
        var entities = await _context.Set<T>().ToListAsync().ConfigureAwait(false);
        if (entities.Count() > 0)
        {
            return Ok(entities);
        }

        return NoContent();
    }

    /// <summary>
    /// Get T by Id
    /// </summary>
    /// <param name="id">T Id</param>
    /// <returns>T</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Get {T} By Id: {Id}", typeof(T).Name, id);

        var entity = await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
        if (entity == null)
        {
            return NotFound();
        }
        return entity;
    }

    /// <summary>
    /// Create T
    /// </summary>
    /// <param name="entity">T</param>
    /// <returns>T</returns>
    [HttpPost]
    public async Task<ActionResult<T>> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return Ok(entity);
    }

    /// <summary>
    /// Update T
    /// </summary>
    /// <param name="id">T Id</param>
    /// <param name="entity">T</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, T entity)
    {
        if (id != GetId(entity))
        {
            return BadRequest();
        }
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }

    /// <summary>
    /// Delete T
    /// </summary>
    /// <param name="id">T Id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async virtual Task<IActionResult> DeleteAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
        if (entity == null)
        {
            return NotFound();
        }
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual int GetId(T entity)
    {
        // Implement this method in derived classes to extract the entity ID
        // Example: return entity.Id;
        throw new System.NotImplementedException();
    }
}