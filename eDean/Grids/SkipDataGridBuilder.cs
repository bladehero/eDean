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
    class SkipDataGridBuilder : DataGridBuilder
    {
        public SkipDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            DataGrid.CanUserAddRows = false;
            DataGrid.CanUserDeleteRows = false;
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Id") }, "ID Студента", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.LastName") }, "Фамилия", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Name") }, "Имя", true);
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Student.Patronymic") }, "Отчество", true);
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("ZeroPair") }, "0-ая пара");
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("FirstPair") }, "1-ая пара");
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("SecondPair") }, "2-ая пара");
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("ThirdPair") }, "3-ая пара");
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("FourthPair") }, "4-ая пара");
            BuildColumn(new DataGridCheckBoxColumn() { Binding = new Binding("FifthPair") }, "5-ая пара");

        }
        public override void Remove(object skip)
        {
            var item = skip as Skip;
            Source.Remove(item);
        }
        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Skips.Remove(dgr.Item as Skip);
                }
            }
        }
    }
}
