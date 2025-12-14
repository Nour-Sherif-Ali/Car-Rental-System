using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.Pagination
{
    public class CarQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Brand { get; set; }
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? Available { get; set; }
        public string? SortBy { get; set; } // name, brand, price, createdat
        public bool SortDescending { get; set; } = false;
    }
}
