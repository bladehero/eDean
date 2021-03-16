using DeanDb;
using eDean.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace eDean.Grids
{
    sealed class SubjectDataGridBuilder : DataGridBuilder
    {
        public SubjectDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(new DataGridTextColumn() { Binding = new Binding("Name") }, "Предмет");
        }
        public override void Remove(object subject)
        {
            var item = subject as Subject;
            Source.Remove(item);
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Subjects.Remove(dgr.Item as Subject);
                }
            }
        }
    }
}
