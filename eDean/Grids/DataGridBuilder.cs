using eDean.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.Entity;
using System.Collections;

namespace eDean.Grids
{
    abstract class DataGridBuilder
    {
        private IList source;

        public IList Source
        {
            get => source;
            internal set
            {
                DataGrid.ItemsSource = null;
                source = value;
                DataGrid.ItemsSource = value;
            }
        }
        public DataGrid DataGrid { get; private set; }

        public DataGridBuilder(DataGrid dataGrid)
        {
            dataGrid.AutoGenerateColumns = false;
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            DataGrid = dataGrid;
            DataGrid.PreviewKeyDown += DataGrid_DeleteKeyDown;
        }

        public abstract void Remove(object item);
        public abstract void CreateDataGrid();

        public void BuildIdColumn()
        {
            DataGrid.Columns.Add(new DataGridTextColumn
            {
                IsReadOnly = true,
                Header = "ID",
                MinWidth = 20,
                MaxWidth = 50,
                Width = 35,
                Binding = new Binding("Id")
            });
        }
        public void BuildColumn(DataGridColumn column, string header, bool isReadOnly = false)
        {
            column.Header = header;
            column.IsReadOnly = isReadOnly;
            DataGrid.Columns.Add(column);
        }

        protected DataGridComboBoxColumn CreateTeachersColumn()
        {
            var teachers = new DataGridComboBoxColumn()
            {
                TextBinding = new Binding("TeacherId"),
                ItemsSource = Data.Context.Teachers.Select(d => d.Id).ToList()
            };

            var style = new Style(typeof(ComboBox));
            style.Setters.Add(new EventSetter(ComboBox.MouseEnterEvent,
                new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    var cb = (sender as ComboBox);
                    if (String.IsNullOrWhiteSpace(cb.Text))
                        return;
                    int id = Convert.ToInt32(cb.Text);
                    var teacher = Data.Context.Teachers.FirstOrDefault(d => d.Id == id);
                    cb.ToolTip = new ToolTip()
                    {
                        Content = teacher?.ToString()
                    };
                })));
            teachers.EditingElementStyle = style;

            return teachers;
        }
        protected DataGridComboBoxColumn CreateGroupsColumn()
        {
            var groups = new DataGridComboBoxColumn()
            {
                TextBinding = new Binding("GroupId"),
                ItemsSource = Data.Context.Groups.Select(d => d.Id).ToList()
            };

            var style = new Style(typeof(ComboBox));
            style.Setters.Add(new EventSetter(ComboBox.MouseEnterEvent,
                new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    var cb = (sender as ComboBox);
                    if (String.IsNullOrWhiteSpace(cb.Text))
                        return;
                    int id = Convert.ToInt32(cb.Text);
                    var group = Data.Context.Groups.Include(g => g.Faculty).FirstOrDefault(d => d.Id == id);
                    cb.ToolTip = new ToolTip()
                    {
                        Content = group?.ToString()
                    };
                })));
            groups.EditingElementStyle = style;

            return groups;
        }
        protected DataGridComboBoxColumn CreateSubjectsColumn()
        {
            var subjects = new DataGridComboBoxColumn()
            {
                TextBinding = new Binding("SubjectId"),
                ItemsSource = Data.Context.Subjects.Select(d => d.Id).ToList()
            };

            var style = new Style(typeof(ComboBox));
            style.Setters.Add(new EventSetter(ComboBox.MouseEnterEvent,
                new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    var cb = (sender as ComboBox);
                    if (String.IsNullOrWhiteSpace(cb.Text))
                        return;
                    int id = Convert.ToInt32(cb.Text);
                    var subject = Data.Context.Subjects.FirstOrDefault(d => d.Id == id);
                    cb.ToolTip = new ToolTip()
                    {
                        Content = subject?.Name
                    };
                })));
            subjects.EditingElementStyle = style;

            return subjects;
        }
        protected DataGridComboBoxColumn CreateFacultiesColumn()
        {
            var facultiesId = new DataGridComboBoxColumn()
            {
                TextBinding = new Binding("FacultyId"),
                ItemsSource = Data.Context.Faculties.Select(f => f.Id).ToList()
            };
            var style = new Style(typeof(ComboBox));
            style.Setters.Add(new EventSetter(ComboBox.MouseEnterEvent,
                new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    var cb = (sender as ComboBox);
                    int id = Convert.ToInt32(cb.Text);
                    var faculty = Data.Context.Faculties.FirstOrDefault(t => t.Id == id);
                    cb.ToolTip = new ToolTip()
                    {
                        Content = faculty?.Name
                    };
                })));
            facultiesId.EditingElementStyle = style;
            return facultiesId;
        }
        protected DataGridComboBoxColumn CreateDeansColumn()
        {
            var deansId = new DataGridComboBoxColumn()
            {
                TextBinding = new Binding("DeanId"),
                ItemsSource = Data.Context.Deans.Select(d => d.Id).ToList()
            };

            var style = new Style(typeof(ComboBox));
            style.Setters.Add(new EventSetter(ComboBox.MouseEnterEvent,
                new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    var cb = (sender as ComboBox);
                    int id = Convert.ToInt32(cb.Text);
                    var dean = Data.Context.Deans.FirstOrDefault(d => d.Id == id);
                    cb.ToolTip = new ToolTip()
                    {
                        Content = dean
                    };
                })));
            deansId.EditingElementStyle = style;

            return deansId;
        }
        protected abstract void DataGrid_DeleteKeyDown(object sender, KeyEventArgs e);
    }
}
