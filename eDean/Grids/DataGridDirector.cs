using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace eDean.Grids
{
    class DataGridDirector
    {
        public DataGrid Create(DataGridBuilder builder, IList source)
        {
            builder.CreateDataGrid();
            builder.Source = source;
            return builder.DataGrid;
        }
    }
}
