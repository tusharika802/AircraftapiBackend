using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Aircraftapi.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        [AllowNull]
        public ICollection<Partner> ContractPartners { get; set; } = [];

        [NotMapped]
        public string PartnerNames =>
            ContractPartners != null && ContractPartners.Any()
                ? string.Join(", ", ContractPartners.Select(p => p.Name))
                : string.Empty;
    }
}
