﻿namespace ShopApp.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string? UserAvatar { get; set; }
        public string UserEmail { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool UserGender { get; set; } = true;
        public bool UserActive { get; set; } = true;
        public int? UserCount { get; set; }
        public DateTime? UserCurrentTime { get; set; }
        public DateTime? UserUnlockTime { get; set; }
        public List<string> RoleNames { get; set; }
    }
}
