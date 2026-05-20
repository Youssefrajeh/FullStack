using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal OrderAmount { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Navigation property
        public virtual Customer? Customer { get; set; }
    }
}