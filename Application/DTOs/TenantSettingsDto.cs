using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TenantSettingsDto
    {
        public int Id { get; set; }
        public string? Logo { get; set; }
        // General Information
        public string? Language { get; set; }
        public string? Timezone { get; set; }
        public string? Currency { get; set; }

        // Notification Preferences
        public bool EnableDesktopNotification { get; set; }
        public bool ProjectTaskUpdates { get; set; }
        public bool ApprovalChangeRequests { get; set; }
        public bool SystemAnnouncements { get; set; }
    }
}
