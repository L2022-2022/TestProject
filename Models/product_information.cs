using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Models
{
    public class product_information
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [ForeignKey("id")]
        public virtual product table_product { get; set; }
        public string description { get; set; }
        public string product_category { get; set; }
    }
}
