using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Subject
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; }

        public Subject()
        {
            Courses = new List<Course>();
        }
    }
}
