using DeanDb;
using eDean.Settings;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    sealed class TeacherDataGridBuilder:DataGridBuilder
    {
        public TeacherDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Name") }, "Имя");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("LastName") }, "Фамилия");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Patronymic") }, "Отчество");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Phone") }, "Телефон");
        }
        public override void Remove(object teacher)
        {
            var item = teacher as Teacher;
            Source.Remove(item);
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Teachers.Remove(dgr.Item as Teacher);
                }
            }
        }
    }
}
