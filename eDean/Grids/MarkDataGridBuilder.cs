using DeanDb;
using eDean.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    class MarkDataGridBuilder : DataGridBuilder
    {
        public MarkDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            DataGrid.CanUserAddRows = false;
            DataGrid.CanUserDeleteRows = false;
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Id") }, "ID Студента", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.LastName") }, "Фамилия", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Name") }, "Имя", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Patronymic") }, "Отчество", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Value") }, "Оценка");
        }
        public override void Remove(object mark)
        {
            var item = mark as Mark;
            Source.Remove(item);
        }
        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Marks.Remove(dgr.Item as Mark);
                }
            }
        }
    }
}
