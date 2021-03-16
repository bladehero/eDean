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

namespace eDean.Tabs
{
    public partial class SkipsWindow : Window
    {
        DataGridBuilder builder;
        Group group;
        List<Student> students;
        DateTime date;

        public Mark Selected { get; private set; }

        private void PropertyChanged()
        {
            if (group == null)
            {
                if (builder != null)
                    builder.Source = null;
                return;
            }

            var skips = Data.Context.Skips
                .Include(s => s.Student)
                .Include(s => s.Student.Group)
                .Where(s => (s.Date == date && s.Student.GroupId == group.Id)).ToList();

            var allSkips = new List<Skip>();
            if (skips.Count < students.Count)
            {
                for (int i = 0; i < students.Count; i++)
                {
                    var skip = new Skip
                    {
                        Student = students[i],
                        Date = date,
                        StudentId = students[i].Id,
                    };
                    allSkips.Add(skip);
                }
                for (int i = 0; i < skips.Count; i++)
                {
                    for (int j = 0; j < allSkips.Count; j++)
                    {
                        if (allSkips[j].StudentId == skips[i].StudentId)
                        {
                            allSkips[j] = skips[i];
                        }
                    }
                }
            }
            else
            {
                allSkips = skips;
            }
            builder.Source = allSkips.OrderBy(m => m.Student.LastName).ToList();
        }

        public SkipsWindow()
        {
            InitializeComponent();
            builder = new SkipDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, null);
            groupCb.ItemsSource = GetGroups();
        }

        private IEnumerable GetGroups()
        {
            var groups = Data.Context.Groups.Include(g => g.Faculty).ToList();

            List<string> strings = new List<string>();
            foreach (var group in groups)
            {
                var str = group.ToString();
                if (!strings.Contains(str))
                    strings.Add(str);
            }
            return strings;
        }
        private void DateBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date = (sender as DatePicker).SelectedDate ?? DateTime.MinValue;
            PropertyChanged();
        }
        private void GroupCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

            students = Data.Context.Students.Where(s => s.GroupId == group.Id).OrderBy(s => s.LastName).ToList();
            PropertyChanged();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var skip = dataGrid.SelectedItem as Skip;
            // mark = Data.Context.Marks.Where(m => m.Id == mark.Id).FirstOrDefault();
            if (skip == null || skip.Id == 0)
                return;

            Data.Context.Skips.Remove(skip);
            Data.Context.SaveChanges();
            PropertyChanged();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newSkips = (builder.Source as List<Skip>).Where(m => m.Id == 0).ToList();
            Data.Context.Skips.AddRange(newSkips);
            Data.Context.SaveChanges();
        }
    }
}
