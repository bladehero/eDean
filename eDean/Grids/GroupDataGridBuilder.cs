using DeanDb;
using eDean.Settings;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    sealed class GroupDataGridBuilder : DataGridBuilder
    {
        public GroupDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Number") }, "Номер группы");
            BuildColumn(CreateFacultiesColumn(), "ID Факультета");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("AdditionalNumber") }, "Доп. номер");
            BuildColumn(CreateTeachersColumn(), "ID Куратора");
        }
        public override void Remove(object group)
        {
            var item = group as Group;
            if (item == null) return;
            Data.Context.Groups.Remove(item);
            Source.Remove(item);
            DataGrid.Items.Refresh();
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Groups.Remove(dgr.Item as Group);
                }
            }
        }
    }
}