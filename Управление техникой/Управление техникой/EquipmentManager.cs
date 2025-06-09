using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Управление_техникой
{
    public class EquipmentManager
    {
        private List<Equipment> equipments = new List<Equipment>();
        private ListView listView;
        public EquipmentManager(ListView listView)
        {
            this.listView = listView;
            this.listView.View = View.Details;
            this.listView.FullRowSelect = true;
            this.listView.Columns.Add("Имя", 150);
            this.listView.Columns.Add("Тип", 100);
            this.listView.Columns.Add("Серийный номер", 150);
            this.listView.Columns.Add("Состояние", 100);
        }
        public void AddEquipment(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                {
                    MessageBox.Show("Объект оборудования не создан.");
                    return;
                }
                // Проверка на дубликаты серийного номера
                if (equipments.Any(e => e.SerialNumber == equipment.SerialNumber))
                {
                    MessageBox.Show("Оборудование с таким серийным номером уже существует.");
                    return;
                }
                if (equipment.PurchaseDate > equipment.LastMaintenanceDate)
                {
                    MessageBox.Show("Дата покупки не может быть позже даты последнего обслуживания.");
                    return;
                }
                equipments.Add(equipment);
                CheckEquipmentStatus(equipment);
                MessageBox.Show("Оборудование успешно добавлено.");
                LoadEquipment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении оборудования: {ex.Message}");
            }
        }
        public void RemoveEquipment(string serialNumber)
        {
            try
            {
                var equipmentToRemove = equipments.Find(e => e.SerialNumber ==
                serialNumber);
                if (equipmentToRemove != null)
                {
                    equipments.Remove(equipmentToRemove);
                    MessageBox.Show("Оборудование успешно удалено.");
                    LoadEquipment();
                }
                else
                {
                    MessageBox.Show("Оборудование с таким серийным номером не найдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении оборудования: {ex.Message}");
            }
        }
        private void CheckEquipmentStatus(Equipment equipment)
        {
            if (equipment.InMaintenanceOverdue())
            {
                MessageBox.Show(
                    $"Внимание! Добавляемое оборудование '{equipment.Name}' (№{equipment.SerialNumber}) имеет просроченное ТО и нуждается в обсуживании\n" +
                    $"Последнее обслуживание: {equipment.LastMaintenanceDate:dd.MM.yyyy}",
                    "Предупреждение о состоянии оборудования",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else if (equipment.InMaintenanceDue())
            {
                MessageBox.Show(
                    $"Оборудование '{equipment.Name}' (№{equipment.SerialNumber}) требует планового ТО.\n" +
                    $"Последнее обслуживание: {equipment.LastMaintenanceDate:dd.MM.yyyy}",
                    "Информация о состоянии оборудования",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void LoadEquipment()
        {
            try
            {
                listView.Items.Clear();
                if (equipments == null || equipments.Count == 0)
                {
                    MessageBox.Show("Список оборудования пуст.");
                    return;
                }
                foreach (var equip in equipments)
                {
                    listView.Items.Add(new ListViewItem(new[]
                    {
                        equip.Name,
                        equip.Type,
                        equip.SerialNumber,
                        equip.Status.ToString()
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке оборудования: {ex.Message}");
            }
        }

        public List<Equipment> GetEquipmentDueForMaintenance()
        {
            return equipments.Where(e => e.InMaintenanceDue()).ToList();
        }

        public List<Equipment> GetEquipmentOverdueMaintenance()
        {
            return equipments.Where(e => e.InMaintenanceOverdue()).ToList();
        }
        public void CheckMaintenanceNotifications()
        {
            var dueEquipment = GetEquipmentDueForMaintenance();
            var overdueEquipment = GetEquipmentOverdueMaintenance();

            // Формируем подробное сообщение
            var message = new System.Text.StringBuilder();

            if (overdueEquipment.Any())
            {
                message.AppendLine("=== ВНИМАНИЕ: Оборудование с просроченным обслуживанием ===");
                foreach (var equip in overdueEquipment)
                {
                    //var overdueDays = (DateTime.Now - equip.LastMaintenanceDate.AddMonths(equip.MaintenanceIntervalMonths)).Days;
                    message.AppendLine($"- {equip.Name} (№{equip.SerialNumber})");
                }
                message.AppendLine();
            }

            if (dueEquipment.Any())
            {
                message.AppendLine("=== Оборудование, требующее скорого обслуживания ===");
                foreach (var equip in dueEquipment)
                {
                    //var daysRemaining = (equip.LastMaintenanceDate.AddMonths(equip.MaintenanceIntervalMonths) - DateTime.Now).Days;
                    message.AppendLine($"- {equip.Name} (№{equip.SerialNumber})");
                }
                message.AppendLine();
            }

            if (message.Length > 0)
            {
                MessageBox.Show(message.ToString(), "Уведомления о техническом обслуживании",
                               MessageBoxButtons.OK,
                               overdueEquipment.Any() ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }

        public void SetMaintenanceInterValForAll(int months)
        {
            foreach (var e in equipments) { e.MaintenanceIntervalMonths = months; }
        }
        public Equipment GetEquipmentBySerialNumber(string serialNumber)
        {
            return equipments.FirstOrDefault(e => e.SerialNumber == serialNumber);
        }

        public List<Equipment> GetEquipmentList()
        {
            return equipments;
        }

        public void RefreshEquipmentList()
        {
            LoadEquipment();
        }
        public void DisplayEquipmentInfo()
        {
            try
            {
                if (equipments == null || equipments.Count == 0)
                {
                    MessageBox.Show("Список оборудования пуст.");
                    return;
                }
                var displayInfoForm = new DisplayInfoForm(this);
                displayInfoForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении информации: {ex.Message}");
            }
        }
    }
}
