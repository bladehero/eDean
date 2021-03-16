using DeanDb;
using eDean.Settings;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    class DeanDataGridBuilder : DataGridBuilder
    {
        public DeanDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Name") }, "Имя");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("LastName") }, "Фамилия");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Patronymic") }, "Отчество");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Phone") }, "Телефон");
        }
        public override void Remove(object course)
        {
            var item = course as Dean;
            Source.Remove(item);
        }
        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Deans.Remove(dgr.Item as Dean);
                }
            }
        }
    }
}
