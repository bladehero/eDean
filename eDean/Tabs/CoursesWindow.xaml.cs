using DeanDb;
using eDean.Grids;
using eDean.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace eDean.Tabs
{

    public partial class CoursesWindow : Window
    {
        private List<Course> source = Data.Context.Courses.Include(c=>c.Subject).Include(c=> c.Teacher).ToList();
        private DataGridBuilder builder;

        public Course Selected { get; private set; }

        public CoursesWindow()
        {
            InitializeComponent();
            builder = new CourseDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, source);
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected = (sender as DataGrid).SelectedItem as Course;
            if (Selected == null) return;

            subjectTb.Text = Selected.Subject?.Name;
            groupTb.Text = $"{Selected.Group?.Number.ToString()}{Selected.Group?.Faculty?.Name}{Selected.Group?.AdditionalNumber?.ToString()}";
            lastnameTb.Text = $"Фамилия: {Selected.Teacher?.LastName?.ToString()}";
            nameTb.Text = $"Имя: {Selected.Teacher?.Name?.ToString()}";
            patronymicTb.Text = $"Отчество: {Selected.Teacher?.Patronymic?.ToString()}";
            phoneTb.Text = $"Телефон: {Selected.Teacher?.Phone?.ToString()}";
        }
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            builder.Remove(dataGrid.SelectedItem);
            dataGrid.Items.Refresh();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNewItems();
            Data.Context.SaveChanges();
            dataGrid.Items.Refresh();
        }
        private void SaveNewItems()
        {
            foreach (var item in source)
                if (item.Id == 0)
                    Data.Context.Courses.Add(item);
        }
    }
}
