﻿using MyBGList.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MyBGList.DTO
{
    public class MechanicDTO
    {
        [Required]
        public int Id { get; set; }
        [LettersOnlyValidator(UseRegex = true)]
        public string? Name { get; set; }
    }
}
