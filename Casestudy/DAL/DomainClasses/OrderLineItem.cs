using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class OrderLineItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(450)]
        [ForeignKey("Product")]
        public string? ProductId { get; set; }

        [Required]
        public int QtyOrdered { get; set; }

        [Required]
        public int QtySold { get; set; }

        [Required]
        public int QtyBackOrdered { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }

        // Navigation properties
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}