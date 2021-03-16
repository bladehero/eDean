using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Secretary : User
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
