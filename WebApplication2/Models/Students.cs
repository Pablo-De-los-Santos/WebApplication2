using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Students
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public long? StudentCode { get; set; }
        public string Rne { get; set; }
        public string State { get; set; }
        public string Period { get; set; }
        public string AcademicLevel { get; set; }
        public int? Level { get; set; }
        public string BloodType { get; set; }
        public string ConditionDescription { get; set; }

        public int SchoolId { get; set; }

        [ForeignKey("SchoolId")]
        public virtual Schools School { get; set; }

    }
}
