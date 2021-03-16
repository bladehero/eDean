using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public abstract class User
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public long ChatId { get; set; }

        public override string ToString()
        {
            return $"{Name} {LastName} {Patronymic}";
        }
    }
}
