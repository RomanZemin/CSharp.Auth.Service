﻿namespace Auth.Domain.Entities
{
    public abstract class AuditableEntityBase
    {
        public DateTime CreatedOn { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
