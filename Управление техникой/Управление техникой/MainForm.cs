using System;
using System.Windows.Forms;

namespace Управление_техникой
{
    public partial class MainForm : Form
    {
        private EquipmentManager equipmentManager;
        private TextBox nameTextBox;
        private TextBox typeTextBox;
        private TextBox serialNumberTextBox;
        private DateTimePicker purchaseDatePicker;
        private DateTimePicker lastMaintenanceDatePicker;
        private Label nameLabel;
        private Label typeLabel;
        private Label serialNumberLabel;
        private Label purchaseLabel;
        private Label lastMaintenanceLabel;
        public MainForm()
        {
            this.Text = "Управление оборудованием";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
            var listView = new ListView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(780, 300)
            };
            equipmentManager = new EquipmentManager(listView);
            this.Controls.Add(listView);
        }
        private void InitializeComponents()
        {
            nameLabel = new Label
            {
                Location = new System.Drawing.Point(10, 320),
                Text = "Название:",
            };
            typeLabel = new Label
            {
                Location = new System.Drawing.Point(120, 320),
                Text = "Тип:",
            };
            serialNumberLabel = new Label
            {
                Location = new System.Drawing.Point(230, 320),
                Text = "Серийный номер:",
            };
            purchaseLabel = new Label
            {
                Location = new System.Drawing.Point(340, 320),
                Text = "Дата покупки:",

            };
            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 340),
                Width = 100
            };
            typeTextBox = new TextBox
            {
                Location = new System.Drawing.Point(120, 340),
                Width = 100
            };
            serialNumberTextBox = new TextBox
            {
                Location = new System.Drawing.Point(230, 340),
                Width = 100
            };
            purchaseDatePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(340, 340)
            };
            lastMaintenanceDatePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(purchaseDatePicker.Right + 10, 340)
            };
            lastMaintenanceLabel = new Label
            {
                Location = new System.Drawing.Point(purchaseDatePicker.Right + 10, 320),
                Text = "Дата последнего обслуждживания:",
                AutoSize = true
            };
            var addButton = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(560, 420)
            };
            addButton.Click += AddButton_Click;
            var removeButton = new Button
            {
                Text = "Удалить",
                Location = new System.Drawing.Point(560, 450)
            };
            removeButton.Click += RemoveButton_Click;
            var displayButton = new Button
            {
                Text = "Отобразить информацию",
                Location = new System.Drawing.Point(560, 480)
            };
            displayButton.Click += DisplayButton_Click;
            this.Controls.Add(nameTextBox);
            this.Controls.Add(typeTextBox);
            this.Controls.Add(serialNumberTextBox);
            this.Controls.Add(purchaseDatePicker);
            this.Controls.Add(lastMaintenanceDatePicker);
            this.Controls.Add(nameLabel);
            this.Controls.Add(typeLabel);
            this.Controls.Add(serialNumberLabel);
            this.Controls.Add(purchaseLabel);
            this.Controls.Add(lastMaintenanceLabel);
            this.Controls.Add(addButton);
            this.Controls.Add(removeButton);
            this.Controls.Add(displayButton);
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nameTextBox.Text) &&
            !string.IsNullOrEmpty(typeTextBox.Text) &&
            !string.IsNullOrEmpty(serialNumberTextBox.Text))
            {
                // Проверка дат
                if (purchaseDatePicker.Value > lastMaintenanceDatePicker.Value)
                {
                    MessageBox.Show("Дата покупки не может быть позже даты последнего обслуживания.");
                    return;
                }

                var equipment = new Equipment(
                nameTextBox.Text,
                typeTextBox.Text,
                serialNumberTextBox.Text,
                purchaseDatePicker.Value,
                lastMaintenanceDatePicker.Value,
                EquipmentStatus.InGoodCondition
                );
                MessageBox.Show("Оборудование успешно добавлено.");
                equipmentManager.AddEquipment(equipment);
                Clear();
            }
            else
            {
                MessageBox.Show("Заполните все обязательные поля.");
            }
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(serialNumberTextBox.Text))
            {
                MessageBox.Show("Оборудование успешно удалено.");
                equipmentManager.RemoveEquipment(serialNumberTextBox.Text);
                Clear();
            }
            else
            {
                MessageBox.Show("Введите серийный номер для удаления.");
            }
        }
        private void DisplayButton_Click(object sender, EventArgs e)
        {
            equipmentManager.DisplayEquipmentInfo();
            Clear();
        }

        private void Clear()
        {
            nameTextBox.Text = "";
            typeTextBox.Text = "";
            serialNumberTextBox.Text = "";
            purchaseDatePicker.Value = DateTime.Now;
            lastMaintenanceDatePicker.Value = DateTime.Now;
        }
    }
}
