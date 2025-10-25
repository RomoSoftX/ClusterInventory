namespace ClusterInventory.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("sku")]
        public string Sku { get; set; } = default!;   // Único

        [BsonElement("name")]
        public string Name { get; set; } = default!;

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; } = 0;

        [BsonElement("minStock")]
        public int MinStock { get; set; } = 0;

        [BsonElement("location")]
        public string? Location { get; set; }
    }
}
