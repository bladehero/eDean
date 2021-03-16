using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Course
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Required]
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        ICollection<Mark> Marks { get; set; }
        ICollection<Skip> Skips { get; set; }

        public Course()
        {
            Skips = new List<Skip>();
        }
    }
}
