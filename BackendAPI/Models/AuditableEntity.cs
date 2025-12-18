using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public abstract class AuditableEntity
    {
        // Creation Info 
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property for Creator
        [ForeignKey(nameof(CreatedByUserId))]
        public virtual User? CreatedByUser { get; set; }

        // Update 
        public int? UpdatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property for Updater
        [ForeignKey(nameof(UpdatedByUserId))]
        public virtual User? UpdatedByUser { get; set; }

        // Soft Delete 
        public bool IsActive { get; set; } = true;
    }
}