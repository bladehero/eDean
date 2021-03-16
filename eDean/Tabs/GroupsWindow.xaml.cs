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
    public partial class GroupsWindow : Window
    {
        private List<Group> source = Data.Context.Groups.Include(f => f.Teacher).Include(f => f.Faculty).ToList();
        private DataGridBuilder builder;

        public Group Selected { get; private set; }

        public GroupsWindow()
        {
            InitializeComponent();
            builder = new GroupDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, source);
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected = (sender as DataGrid).SelectedItem as Group;
            if (Selected == null) return;

            facultyNameTb.Text = $"{Selected.Number.ToString()}{Selected.Faculty?.Name}{Selected.AdditionalNumber?.ToString()}";
            teacherLastNameTb.Text = $"Фамилия: {Selected.Teacher?.LastName?.ToString()}";
            teacherNameTb.Text = $"Имя: {Selected.Teacher?.Name?.ToString()}";
            teacherPatronymic.Text = $"Отчество: {Selected.Teacher?.Patronymic?.ToString()}";
            teacherPhone.Text = $"Телефон: {Selected.Teacher?.Phone?.ToString()}";
        }
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            builder.Remove(dataGrid.SelectedItem);
            dataGrid.Items.Refresh();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNewGroups();
            Data.Context.SaveChanges();
            dataGrid.Items.Refresh();
        }
        private void SaveNewGroups()
        {
            foreach (var item in source)
                if (item.Id == 0)
                    Data.Context.Groups.Add(item);
        }
    }
}
