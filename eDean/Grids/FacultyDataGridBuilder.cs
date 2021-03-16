using DeanDb;
using eDean.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    sealed class FacultyDataGridBuilder : DataGridBuilder
    {
        public FacultyDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Name") }, "Факультет");
            BuildColumn(CreateDeansColumn(), "Зав. отделением");
        }
        public override void Remove(object faculty)
        {
            var item = faculty as Faculty;
            if (item == null) return;

            Source.Remove(item);
            Data.Context.Faculties.Remove(item);
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Source.Remove(dgr.Item as Faculty);
                }
            }
        }
    }
}
