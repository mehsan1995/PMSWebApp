using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TenantSettings : ITenant, IAuditable, ISoftDeletable
    {
        public int Id { get; set; }
        public string? Logo { get; set; }
        // General Information
        public string? Language { get; set; }
        public string? Timezone { get; set; }
        public string? Currency { get; set; }

        // Notification Preferences
        public bool? EnableDesktopNotification { get; set; }
        public bool? ProjectTaskUpdates { get; set; }
        public bool? ApprovalChangeRequests { get; set; }
        public bool? SystemAnnouncements { get; set; }
        public int TenantId { get ; set ; }
        public DateTime CreatedAt { get ; set ; }
        public string? CreatedBy { get ; set ; }
        public DateTime? ModifiedAt { get ; set ; }
        public string? ModifiedBy { get ; set ; }
        public bool IsDeleted { get ; set ; }
    }
}
