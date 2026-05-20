using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Product
    {
        [Key]
        [StringLength(450)]
        public string? Id { get; set; }

        [Required]
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string? ProductName { get; set; }

        [Required]
        [StringLength(20)]
        public string? GraphicName { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }

        [Required]
        public int QtyOnHand { get; set; }

        [Required]
        public int QtyOnBackOrder { get; set; }

        [Required]
        [StringLength(2000)]
        public string? Description { get; set; }
    }
}
