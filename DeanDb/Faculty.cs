using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Faculty
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? DeanId { get; set; }
        public Dean Dean { get; set; }
        public ICollection<Group> Groups { get; set; }

        public Faculty()
        {
            Groups = new List<Group>();
        }
    }
}
