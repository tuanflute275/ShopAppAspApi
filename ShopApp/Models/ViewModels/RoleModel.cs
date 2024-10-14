﻿using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class RoleModel
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
    }
}
