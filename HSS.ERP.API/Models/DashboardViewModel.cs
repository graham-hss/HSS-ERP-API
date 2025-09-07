namespace HSS.ERP.API.Models;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<AppModule> AvailableModules { get; set; } = new();
    public List<ActivityItem> RecentActivity { get; set; } = new();
    public DashboardStats QuickStats { get; set; } = new();
}

public class AppModule
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    
    public AppModule(string name, string description, string url, string icon = "📋")
    {
        Name = name;
        Description = description;
        Url = url;
        Icon = icon;
    }
}

public class ActivityItem
{
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    
    public ActivityItem(string action, string description, DateTime timestamp)
    {
        Action = action;
        Description = description;
        Timestamp = timestamp;
    }
}

public class DashboardStats
{
    // Common stats
    public int PersonalBookings { get; set; }
    public int CompletedCourses { get; set; }
    public int CertificatesEarned { get; set; }
    public int UpcomingDeadlines { get; set; }
    
    // Financial Controller stats
    public int TotalInvoices { get; set; }
    public int PendingPayments { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int OverdueInvoices { get; set; }
    
    // Course Administrator stats
    public int TotalCourses { get; set; }
    public int ActiveSchedules { get; set; }
    public int TotalBookings { get; set; }
    public int CapacityUtilization { get; set; }
    
    // Training Coordinator stats
    public int TeamBookings { get; set; }
    public int UpcomingCourses { get; set; }
    public int CompletedTraining { get; set; }
    public int TeamUtilization { get; set; }
}