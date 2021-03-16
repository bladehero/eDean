using DeanDb;
using eDean.Grids;
using eDean.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace eDean.Tabs
{
    public partial class MarksWindow : Window
    {
        private readonly int[] pairs = { 0, 1, 2, 3, 4, 5 };
        private DataGridBuilder builder;
        private DateTime date;
        private Teacher teacher;
        private Group group;
        private Subject subject;
        private Course course;
        private int? numberOfPair = null;
        private List<Student> students = new List<Student>();

        private void PropertyChanged()
        {
            if (group == null || subject == null || numberOfPair == null || course == null)
            {
                if (builder != null)
                    builder.Source = null;
                return;
            }

            var marks = Data.Context.Marks
                .Include(m => m.Course)
                .Include(m => m.Course.Subject)
                .Include(m => m.Student)
                .Include(m => m.Student.Group)
                .Where(m => (m.Date == date && m.Student.GroupId == group.Id
                && m.Course.Subject.Id == subject.Id && m.NumberOfPair == numberOfPair)).ToList();

            var allMarks = new List<Mark>();
            if (marks.Count < students.Count)
            {
                for (int i = 0; i < students.Count; i++)
                {
                    var mark = new Mark
                    {
                        Student = students[i],
                        Course = course,
                        Date = date,
                        NumberOfPair = (int)numberOfPair,
                        CourseId = course.Id,
                        StudentId = students[i].Id
                    };
                    allMarks.Add(mark);
                }
                for (int i = 0; i < marks.Count; i++)
                {
                    for (int j = 0; j < allMarks.Count; j++)
                    {
                        if (allMarks[j].StudentId == marks[i].StudentId)
                        {
                            allMarks[j] = marks[i];
                        }
                    }
                }
            }
            else
            {
                allMarks = marks;
            }
            builder.Source = allMarks.OrderBy(m => m.Student.LastName).ToList();
        }

        public Mark Selected { get; private set; }

        public MarksWindow(Teacher teacher)
        {
            this.teacher = teacher;
            InitializeComponent();
            builder = new MarkDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, null);
            groupCb.ItemsSource = GetGroups();
        }

        private IEnumerable GetGroups()
        {
            var courses = Data.Context.Courses.Where(c => c.TeacherId == teacher.Id).Include(c => c.Group).Include(c => c.Group.Faculty).ToList();

            List<string> strings = new List<string>();
            foreach (var course in courses)
            {
                var str = course.Group.ToString();
                if (!strings.Contains(str))
                    strings.Add(str);
            }
            return strings;
        }
        private void GroupCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            subject = null;
            var value = (sender as ComboBox).SelectedItem.ToString();
            var groups = Data.Context.Groups.Include(g => g.Faculty).ToList();
            foreach (var g in groups)
            {
                if (g.ToString() == value)
                {
                    group = g;
                    break;
                }
            }
            var subjects = Data.Context.Courses.Where(c => c.TeacherId == teacher.Id).Where(c => c.GroupId == group.Id).Select(c => c.Subject).ToList();

            var strings = new List<string>();
            foreach (var subject in subjects)
            {
                strings.Add(subject.Name);
            }
            subjectCb.ItemsSource = strings;

            students = Data.Context.Students.Where(s => s.GroupId == group.Id).OrderBy(s => s.LastName).ToList();
            PropertyChanged();
        }
        private void SubjectCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ComboBox).SelectedItem;
            if (item == null) return;

            var value = item.ToString();
            subject = Data.Context.Subjects.Where(s => s.Name == value).FirstOrDefault();
            course = Data.Context.Courses.Where(c => c.SubjectId == subject.Id && c.TeacherId == teacher.Id && c.GroupId == group.Id).FirstOrDefault();
            numberOfPairCb.ItemsSource = pairs;
            PropertyChanged();
        }
        private void DateBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date = (sender as DatePicker).SelectedDate ?? DateTime.MinValue;
            PropertyChanged();
        }
        private void NumberOfPairCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            numberOfPair = Convert.ToInt32((sender as ComboBox).SelectedItem);
            PropertyChanged();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newMarks = (builder.Source as List<Mark>).Where(m => m.Id == 0 && m.Value != null).ToList();
            Data.Context.Marks.AddRange(newMarks);
            Data.Context.SaveChanges();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var mark = dataGrid.SelectedItem as Mark;
            if (mark == null)
                return;
            Data.Context.Marks.Remove(mark);
            Data.Context.SaveChanges();
            PropertyChanged();
        }
    }
}
