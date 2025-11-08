using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.Entity.DataTables
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }      // offset
        public int Length { get; set; }     // page size
        public SearchInfo? Search { get; set; }
        public List<OrderInfo>? Order { get; set; }
        public List<ColumnInfo>? Columns { get; set; }
    }

    public class SearchInfo
    {
        public string? Value { get; set; }
        public bool Regex { get; set; }
    }

    public class OrderInfo
    {
        public int Column { get; set; }   // índice de columna
        public string Dir { get; set; } = "asc";  // "asc" o "desc"
    }

    public class ColumnInfo
    {
        public string? Data { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public SearchInfo? Search { get; set; }
    }

    public class DataTableResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; } = new();
    }
}
