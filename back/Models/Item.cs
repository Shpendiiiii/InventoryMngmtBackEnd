using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models;

public class Item
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public int _id { get; set; }
    public string ModelName { get; set; }
    public string Manufacturer { get; set; }
    public int ModelYear { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
}