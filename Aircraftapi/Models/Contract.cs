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
    }
}
