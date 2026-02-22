namespace BackendAPI.Models
{
    public abstract class AuditableEntity
    {
        // Creation Info 
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // Update 
        public Guid UpdatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Soft Delete 
        public bool IsActive { get; set; } = true;
    }
}