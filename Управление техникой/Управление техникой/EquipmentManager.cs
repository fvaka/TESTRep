using System;
using System.Collections.Generic;
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
                if (equipment != null)
                {
                    equipments.Add(equipment);
                    LoadEquipment();
                }
                else
                {
                    MessageBox.Show("Объект оборудования не создан.");
                }
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
        public void DisplayEquipmentInfo()
        {
            try
            {
                if (equipments == null || equipments.Count == 0)
                {
                    MessageBox.Show("Список оборудования пуст.");
                    return;
                }
                var displayInfoForm = new DisplayInfoForm();
                displayInfoForm.Equipment = equipments;
                displayInfoForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении информации: {ex.Message}");
            }
        }
    }
}
