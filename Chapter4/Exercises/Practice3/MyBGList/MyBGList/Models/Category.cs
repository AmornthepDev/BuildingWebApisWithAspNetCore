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
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        #region Navigation Property

        public ICollection<BoardGames_Categories>? BoardGames_Categories { get; set; }

        #endregion
    }
}
