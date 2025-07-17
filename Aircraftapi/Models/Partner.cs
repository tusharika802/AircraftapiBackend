using System.ComponentModel.DataAnnotations.Schema;

namespace Aircraftapi.Models
{
    public class Partner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //[ForeignKey("ContractId")]
        //public int? ContractId { get; set; } 
        //public Contract Contract { get; set; }
        public string Email { get; set; }

    }
}
