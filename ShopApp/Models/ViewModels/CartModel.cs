﻿namespace ShopApp.Models.ViewModels
{
    public class CartModel
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
