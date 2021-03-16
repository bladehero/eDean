using eDean.Settings;
using System.Windows.Controls;
using System.Windows.Input;
using DeanDb;

namespace eDean.Grids
{
    sealed class CourseDataGridBuilder : DataGridBuilder
    {
        public CourseDataGridBuilder(DataGrid dataGrid) : base(dataGrid) { }

        public override void CreateDataGrid()
        {
            BuildIdColumn();
            BuildColumn(CreateSubjectsColumn(), "ID Предмета");
            BuildColumn(CreateTeachersColumn(), "ID Преподавателя");
            BuildColumn(CreateGroupsColumn(), "ID Группы");
        }
        public override void Remove(object course)
        {
            var item = course as Course;
            Source.Remove(item);
        }

        protected override void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg != null)
            {
                var dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    Data.Context.Courses.Remove(dgr.Item as Course);
                }
            }
        }
    }
}
