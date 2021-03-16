using DeanDb;
using eDean.Grids;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using eDean.Settings;

namespace eDean.Tabs
{
    public partial class FacultiesWindow : Window
    {
        private List<Faculty> source = Data.Context.Faculties.Include(f => f.Dean).ToList();
        private DataGridBuilder builder;

        public Faculty Selected { get; private set; }

        public FacultiesWindow()
        {
            InitializeComponent();
            builder = new FacultyDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, source);
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selected = (sender as DataGrid).SelectedItem as Faculty;
            if (Selected == null) return;

            deanLastNameTb.Text = $"Фамилия: {Selected.Dean?.LastName?.ToString()}";
            deanNameTb.Text = $"Имя: {Selected.Dean?.Name?.ToString()}";
            deanPatronymic.Text = $"Отчество: {Selected.Dean?.Patronymic?.ToString()}";
            deanPhone.Text = $"Телефон: {Selected.Dean?.Phone?.ToString()}";
        }
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            builder.Remove(dataGrid.SelectedItem);
            dataGrid.Items.Refresh();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNewFaculties();
            try
            {
                Data.Context.SaveChanges();
            }
            catch (System.Exception) { }
            dataGrid.Items.Refresh();
        }
        private void SaveNewFaculties()
        {
            foreach (var item in source)
                if (item.Id == 0)
                    Data.Context.Faculties.Add(item);
        }
    }
}
