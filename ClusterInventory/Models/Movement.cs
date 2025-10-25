namespace ClusterInventory.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    public class Movement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("itemId")]
        public string ItemId { get; set; } = default!;

        // "in" o "out"
        [BsonElement("type")]
        public string Type { get; set; } = default!;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("note")]
        public string? Note { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
