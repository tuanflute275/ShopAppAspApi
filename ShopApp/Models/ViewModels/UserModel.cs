﻿using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class UserModel
    {
        [Required]
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? OldImage { get; set; }
        [EmailAddress]
        public string UserEmail { get; set; }
        [MinLength(6)]
        public string? UserPassword { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool UserGender { get; set; } = true;
        public bool UserActive { get; set; } = true;
        public string? Role { get; set; }
    }
}
