using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Student : User
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int? GroupId { get; set; }
        public Group Group { get; set; }

        ICollection<Mark> Marks { get; set; }
        ICollection<Skip> Skips { get; set; }

        public Student()
        {
            Marks = new List<Mark>();
            Skips = new List<Skip>();
        }
    }
}
