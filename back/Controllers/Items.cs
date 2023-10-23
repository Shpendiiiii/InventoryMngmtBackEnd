using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using WebApplication1.Models;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/items")]
public class Items : Controller
{
    private readonly IMongoCollection<Item> itemsCollection;

    private readonly ILogger<Items> _logger;

    public Items(IConfiguration config, ILogger<Items> logger)
    {
        _logger = logger;
        var connectionString = config.GetConnectionString("Db");
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("IM");
        itemsCollection = database.GetCollection<Item>("Product");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        try
        {
            var items = await itemsCollection.Find(item => true).ToListAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving items from MongoDB");
            // Handle exceptions and return an appropriate response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostUserInput([FromBody] Item item)
    {
        try
        {
            await itemsCollection.InsertOneAsync(item);
            return Ok("User input successfullu was entered in the databse");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteItems([FromQuery]List<int> itemIds)
    {
        if (itemIds == null || !itemIds.Any())
        {
            return BadRequest("No items selected for deletion.");
        }

        var filter = Builders<Item>.Filter.In(item => item._id, itemIds);
        var result = await itemsCollection.DeleteManyAsync(filter);

        if (result.DeletedCount > 0)
        {
            return Ok($"Successfully deleted {result.DeletedCount} items.");
        }
        else
        {
            return NotFound("No items were deleted.");
        }
    }
}