using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Group
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }
        public int? AdditionalNumber { get; set; }

        [Required]
        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }
        
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        ICollection<Student> Students { get; set; }
        ICollection<Course> Courses { get; set; }

        public Group()
        {
            Students = new List<Student>();
            Courses = new List<Course>();
        }

        public override string ToString()
        {
            return $"{Number}{Faculty.Name}{AdditionalNumber?.ToString()}";
        }
    }
}
