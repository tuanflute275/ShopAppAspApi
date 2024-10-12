﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.Entities
{
    [Table("couponCondition")]
    public class CouponCondition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        [Range(0, int.MaxValue)]
        public int CouponId { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [Required]
        public string Attribute { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required]
        public string Operator { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [Required]
        public string Value { get; set; }

        [Column(name: "discountAmount")]
        [Range(0, double.MaxValue)]
        [Required]
        public double DiscountAmount { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }
    }
}