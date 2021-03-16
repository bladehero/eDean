using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeanDb
{
    public class Skip
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool ZeroPair { get; set; }
        public bool FirstPair { get; set; }
        public bool SecondPair { get; set; }
        public bool ThirdPair { get; set; }
        public bool FourthPair { get; set; }
        public bool FifthPair { get; set; }
    }
}
