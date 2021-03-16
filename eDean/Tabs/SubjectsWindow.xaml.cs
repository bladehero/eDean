using DeanDb;
using eDean.Grids;
using eDean.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace eDean.Tabs
{
    public partial class SubjectsWindow : Window
    {
        private List<Subject> source = Data.Context.Subjects.ToList();
        private DataGridBuilder builder;

        public SubjectsWindow()
        {
            InitializeComponent();
            builder = new SubjectDataGridBuilder(dataGrid);
            new DataGridDirector().Create(builder, source);
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
                    Data.Context.Subjects.Add(item);
        }
    }
}
