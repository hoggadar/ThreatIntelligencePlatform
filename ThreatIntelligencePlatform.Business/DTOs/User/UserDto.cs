﻿namespace ThreatIntelligencePlatform.Business.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string[] Roles { get; set; } = null!;
    }
}
