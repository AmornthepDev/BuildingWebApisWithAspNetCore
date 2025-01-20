using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBGList.Models
{
    [Table("Publishers")]
    public class Publisher
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        #region
        public ICollection<BoardGame> BoardGames { get; set; } = new List<BoardGame>();
        #endregion
    }
}
