using DeanDb;
using eDean.Settings;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    sealed class StudentDataGridBuilder : DataGridBuilder
    {
        public StudentDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Name") }, "Имя");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("LastName") }, "Фамилия");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Patronymic") }, "Отчество");
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Phone") }, "Телефон");
            BuildColumn(CreateGroupsColumn(), "ID Группы");
        }
        public override void Remove(object student)
        {
            var item = student as Student;
            Source.Remove(item);
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Students.Remove(dgr.Item as Student);
                }
            }
        }
    }
}
