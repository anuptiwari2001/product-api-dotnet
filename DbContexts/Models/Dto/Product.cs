using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductAPI.DbContexts.Models.Dto
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Range(0, 1000)]
        public required string Description { get; set; }

        public required string CategoryName { get; set; }

        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
