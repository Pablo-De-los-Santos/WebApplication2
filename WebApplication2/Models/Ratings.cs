using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Ratings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RatingId { get; set; }
        public int? StudentId { get; set; }

        public string Course { get; set; }
        public int? Score { get; set; }

        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }
    }
}
