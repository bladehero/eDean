using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Teacher : User
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ICollection<Group> Groups { get; set; }
        public ICollection<Course> Courses { get; set; }

        public Teacher()
        {
            Groups = new List<Group>();
            Courses = new List<Course>();
        }
    }
}
