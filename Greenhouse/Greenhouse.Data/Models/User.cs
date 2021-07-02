using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Greenhouse.Data.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Ключ доступа для контроллера.
        /// </summary>
        [Required]
        public Guid ControllerAccessToken { get; set; } = Guid.NewGuid();
    }
}