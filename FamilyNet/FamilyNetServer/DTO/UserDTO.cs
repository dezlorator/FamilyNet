using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNetServer.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
