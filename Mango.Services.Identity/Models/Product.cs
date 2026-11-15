using System.ComponentModel.DataAnnotations;

namespace Mango.Services.Identity.Models;

public sealed class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public required string Description { get; set; }

    [Required]
    public required string CategoryName { get; set; }

    [Range(0, 1000)]
    public double Price { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}
