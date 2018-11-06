using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmanagerService.Core.Entities
{
    public class Company
    {
        [Key]
        [StringLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        public string Logo { get; set; }

        public string AdminId { get; set; }

        public bool IsDepartmentOfConstruction { get; set; }

        public bool Status { get; set; }
    }
}
