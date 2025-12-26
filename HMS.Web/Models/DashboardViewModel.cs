namespace HMS.Web.Models;

public class DashboardViewModel
{
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int TotalBeds { get; set; }
    public int AvailableBeds { get; set; }
    public int OccupiedBeds { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public int PendingBills { get; set; }
    public int LowStockMedicines { get; set; }
    public int PendingLabTests { get; set; }
}