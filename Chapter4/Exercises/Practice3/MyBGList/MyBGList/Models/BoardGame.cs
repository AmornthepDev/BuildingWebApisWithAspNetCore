﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("BoardGames")]
    public class BoardGame
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        [Required]
        public int MinPlayer { get; set; }

        [Required]
        public int MaxPlayer { get; set; }

        [Required]
        public int PlayTime { get; set; }

        [Required]
        public int MinAge { get; set; }

        [Required]
        public int UsersRated { get; set; }

        [Required]
        [Precision(4, 2)]
        public decimal RatingAverage { get; set; }

        [Required]
        public int BGGRank { get; set; }

        [Required]
        [Precision(4, 2)]
        public decimal ComplexityAverage { get; set; }

        [Required]
        public int OwnedUsers { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }
        [MaxLength(200)]
        public string AlternateNames { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Designer { get; set; } = string.Empty;
        [Required]
        public int Flags { get; set; }

        [Required]
        public int PublisherId { get; set; }

        #region Navigation Property

        public Publisher? Publisher { get; set; }
        public ICollection<BoardGames_Categories>? BoardGames_Categories { get; set; }
        public ICollection<BoardGames_Domains>? BoardGames_Domains { get; set; }
        public ICollection<BoardGames_Mechanics>? BoardGames_Mechanics { get; set; }

        #endregion

    }
}
