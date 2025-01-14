﻿using FashionAPI.Models.BaseRequest;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class RegisterUserRequest 
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Fullname field is required.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Gender field is required.")]
        public sbyte Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly Birthday { get; set; }
        public string Password { get; set; }
    }
}
