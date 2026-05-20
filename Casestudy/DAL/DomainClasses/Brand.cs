using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casestudy.DAL.DomainClasses
{
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //this is like auto number generation for the id
        public int Id { get; set; }
        [Required]//this field is required and can not be empty 
        [StringLength(200)]
        public string? Name { get; set; }
    }
}
