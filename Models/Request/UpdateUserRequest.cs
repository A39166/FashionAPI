﻿using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class UpdateUserRequest : UuidRequest
    {

        [Required(ErrorMessage = "Fullname field is required.")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Gender field is required.")]
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        [DefaultValue(1)]
        public sbyte Role { get; set; } 
        [DefaultValue(1)]
        public sbyte Status { get; set; }
    }
    public class UpdateUserClientRequest : UuidRequest
    {

        public string Fullname { get; set; }

        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Path { get; set; }
    }
}
