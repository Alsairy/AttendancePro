using System.ComponentModel.DataAnnotations;

namespace Hudur.Shared.Domain.Entities
{
    public abstract class TenantEntity : BaseEntity
    {
        [Required]
        public Guid TenantId { get; set; }
        
        public virtual Tenant? Tenant { get; set; }
    }
}

