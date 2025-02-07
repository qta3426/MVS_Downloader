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
    public class FileDetailModel
    {
        public int? Id { get; set; }
        public int productId { get; set; }
        public int fileId { get; set; }
        public string fileName { get; set; }
        public string fileDescription { get; set; }
        public string sha256 { get; set; }
        public string fileType { get; set; }
        public string architecture { get; set; }
        public string languageCode { get; set; }
        public string languageName { get; set; }
        public DateTime releaseDate { get; set; }
        public string note { get; set; }
        public string bootstrapperDownloadLink { get; set; }
    }
    public class ProductFiles
    {
        public int productId { get; set; }
        public List<FileDetailModel> fileDetailModels { get; set; }
    }

    public class FilesForProductsResponse
    {
        public Dictionary<string, ProductFiles> filesForProducts { get; set; }
    }
}
