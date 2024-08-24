using System.ComponentModel.DataAnnotations;

namespace Auth.Domain.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public Guid Id { get; set; }
    }
}
