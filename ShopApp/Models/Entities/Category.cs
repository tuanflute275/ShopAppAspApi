﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string CategoryName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? CategorySlug { get; set; }

        [Column]
        public bool CategoryStatus { get; set; } = true;

        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
