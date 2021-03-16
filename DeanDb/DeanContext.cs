using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DeanDb
{
    public class DeanContext : DbContext
    {
        public DeanContext(string connection) : base(connection) { }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Dean> Deans { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Skip> Skips { get; set; }
        public DbSet<Unregistered> Unregistered { get; set; }
    }
}
