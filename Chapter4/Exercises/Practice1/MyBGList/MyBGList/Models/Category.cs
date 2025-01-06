using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        #region Navigation Properties

        public ICollection<BoardGames_Categories>? BoardGames_Categories { get; set; }

        #endregion
    }
}
