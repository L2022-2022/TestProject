using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string productname { get; set; }
        public decimal qty { get; set; }
        public string sku { get; set; }
        public decimal price { get; set; }
        public string userid { get; set; }
        public bool isenable { get; set; }
    }
}
