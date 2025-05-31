using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Управление_техникой
{
    public partial class DisplayInfoForm : Form
    {
        public EquipmentManager _equipmentManager;
        private TextBox numberTextBox;
        private TextBox infoTextBox;
        public DisplayInfoForm(EquipmentManager equipmentManager)
        {
            InitializeComponents();
            _equipmentManager = equipmentManager;
            RefreshEquipmentInfo();
        }

        private void InitializeComponents()
        {
            this.Text = "Отобразить информацию";
            this.Width = 795;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;

            infoTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(760, 270),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
            };
            var numberLabel = new Label
            {
                Text = "Введити серийный номер оборудования для изменения:",
                Location = new Point(10, infoTextBox.Bottom + 10),
                AutoSize = true,
            };
            numberTextBox = new TextBox
            {
                Location = new Point(numberLabel.Right + 200, infoTextBox.Bottom + 10),
                Width = 200,
            };
            var GoodCondition = new Button
            {
                AutoSize = true,
                Location = new Point(10, infoTextBox.Bottom + 40),
                Text = "Выполнить обслуживание"
            };
            GoodCondition.Click += GoodCondition_Click;

            var Broken = new Button
            {
                AutoSize = true,
                Location = new Point(GoodCondition.Right + 80, infoTextBox.Bottom + 40),
                Text = "Пометить как неисправное"
            };
            Broken.Click += Broken_Click;

            var InRepair = new Button
            {
                AutoSize = true,
                Location = new Point(Broken.Right + 90, infoTextBox.Bottom + 40),
                Text = "Отправить на ремонт"
            };
            InRepair.Click += InRepair_Click;

            this.Controls.Add(infoTextBox);
            this.Controls.Add(numberTextBox);
            this.Controls.Add(numberLabel);
            this.Controls.Add(GoodCondition);
            this.Controls.Add(Broken);
            this.Controls.Add(InRepair);
        }

        private Equipment FindEquipmentBySerialNumber()
        {
            string serialNum = numberTextBox.Text.Trim();
            if (string.IsNullOrEmpty(serialNum))
            {
                MessageBox.Show("Введите серийный номер оборудования.");
                return null;
            }

            var equipment = _equipmentManager.GetEquipmentBySerialNumber(serialNum);
            if (equipment == null)
            {
                MessageBox.Show("Оборудование с таким серийным номером не найдено.");
            }

            return equipment;
        }
        private void RefreshEquipmentInfo()
        {
            infoTextBox.Clear();
            var equipmentList = _equipmentManager.GetEquipmentList();

            if (equipmentList.Count > 0)
            {
                foreach (var equip in equipmentList)
                {
                    string status = equip.GetMaintenanceStatus();
                    infoTextBox.AppendText(equip.ToString() + Environment.NewLine + Environment.NewLine);
                }
            }
            else
            {
                infoTextBox.Text = "Список оборудования пуст или не задан.";
            }
        }

        private void RefreshInfo()
        {
            // Очищаем текстовое поле
            infoTextBox.Clear();

            // Получаем актуальный список оборудования из менеджера
            var equipmentList = _equipmentManager.GetEquipmentList();

            // Проверяем, есть ли оборудование для отображения
            if (equipmentList == null || equipmentList.Count == 0)
            {
                infoTextBox.Text = "Список оборудования пуст или не задан.";
                return;
            }

            // Формируем информацию о каждом оборудовании
            foreach (var equip in equipmentList)
            {
                infoTextBox.AppendText(equip.ToString() + Environment.NewLine + Environment.NewLine);
            }

            // Прокручиваем текстовое поле в начало
            infoTextBox.SelectionStart = 0;
            infoTextBox.ScrollToCaret();
        }
        private void GoodCondition_Click(object sender, EventArgs e)
        {
            var equipment = FindEquipmentBySerialNumber();
            if (equipment != null)
            {
                equipment.PerformMaintenance();
                _equipmentManager.RefreshEquipmentList();
                RefreshEquipmentInfo();
            }
        }

        private void Broken_Click(object sender, EventArgs e)
        {
            var equipment = FindEquipmentBySerialNumber();
            if (equipment != null)
            {
                equipment.MarkAsBroken();
                _equipmentManager.RefreshEquipmentList();
                RefreshEquipmentInfo();
            }
        }

        private void InRepair_Click(object sender, EventArgs e)
        {
            var equipment = FindEquipmentBySerialNumber();
            if (equipment != null)
            {
                equipment.MarkAsInRepair();
                _equipmentManager.RefreshEquipmentList();
                RefreshEquipmentInfo();
            }
        }
    }
}
