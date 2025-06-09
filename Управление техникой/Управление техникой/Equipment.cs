using System;
using System.Windows.Forms;
public class Equipment
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string SerialNumber { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime LastMaintenanceDate { get; set; }
    public EquipmentStatus Status { get; set; }
    public int MaintenanceIntervalMonths { get; set; } = 6;

    public Equipment(string name, string type, string serialNumber, DateTime purchaseDate,
    DateTime lastMaintenanceDate, EquipmentStatus status)
    {
        Name = name;
        Type = type;
        SerialNumber = serialNumber;
        PurchaseDate = purchaseDate;
        LastMaintenanceDate = lastMaintenanceDate;
        Status = status;
    }
   
    public bool InMaintenanceOverdue()
    {
        return DateTime.Now > LastMaintenanceDate.AddMonths(MaintenanceIntervalMonths + 1); 
    }
    
    public bool InMaintenanceDue()
    {
        return DateTime.Now >= LastMaintenanceDate.AddMonths(MaintenanceIntervalMonths); 
    }
    public override string ToString()
    {
        return $"Оборудование: {Name}; Тип: {Type}; Серийный номер: {SerialNumber};"
        +
        $"Дата покупки: {PurchaseDate.ToString("dd.MM.yyyy")}; Дата последнего обслуживания: {LastMaintenanceDate.ToString("dd.MM.yyyy")}; Состояние: {Status}";
    }
    public void PerformMaintenance()
    {
        LastMaintenanceDate = DateTime.Now;
        Status = EquipmentStatus.InGoodCondition;
        MessageBox.Show($"Обслуживание оборудования '{Name}' выполнено.");
    }
    public void MarkAsBroken()
    {
        Status = EquipmentStatus.Broken;
        MessageBox.Show($"Оборудование '{Name}' помечено как неисправное.");
    }
    public void MarkAsInRepair()
    {
        Status = EquipmentStatus.InRepair;
        MessageBox.Show($"Оборудование '{Name}' отправлено в ремонт.");
    }

    public string GetMaintenanceStatus()
    {
        if (InMaintenanceOverdue()) return "Обслуживание просрочено.";
        else if (InMaintenanceDue()) return "Необходимо выполнить обслуживание";
        return "Оборудование не требует обслуживания";
    }
}
public enum EquipmentStatus
{
    InGoodCondition,
    Broken,
    InRepair
}