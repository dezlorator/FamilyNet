using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FamilyNetServer.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List <string> Roles { get; set; }
        public List <string> AllRoles { get; set; }
    }
}
