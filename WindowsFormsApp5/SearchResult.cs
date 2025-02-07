using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
    public class Product
    {
        public string productName { get; set; }  // JSON의 'productName'과 동일
        public int productId { get; set; }
        public string displayName { get; set; }
        public string productVersion { get; set; }
        public string productFamily { get; set; }
        public string latestReleaseDate { get; set; }
        public double score { get; set; }
        public bool isUserEligible { get; set; }
    }
    public class SearchResult
    {
        public List<Product> searchResultsGroupByProduct { get; set; }
    }
}
