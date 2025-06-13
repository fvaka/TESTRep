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
        private Label list;
        private Label notifications;
        private ComboBox notificationsCmb;
        public MainForm()
        {
            this.Text = "Управление оборудованием";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
            var listView = new ListView
            {
                Location = new System.Drawing.Point(10, 20),
                Size = new System.Drawing.Size(780, 300)
            };
            equipmentManager = new EquipmentManager(listView);
            this.Controls.Add(listView);
            var list = new Label
            {
                Text = "Список оборудования:",
                Location = new System.Drawing.Point(10, 5),
                AutoSize = true,
            };
            this.Controls.Add(list);
        }
        private void InitializeComponents()
        {
            nameLabel = new Label
            {
                Location = new System.Drawing.Point(10, 330),
                Text = "Название:",
            };
            typeLabel = new Label
            {
                Location = new System.Drawing.Point(120, 330),
                Text = "Тип:",
            };
            serialNumberLabel = new Label
            {
                Location = new System.Drawing.Point(230, 330),
                Text = "Серийный номер:",
            };
            purchaseLabel = new Label
            {
                Location = new System.Drawing.Point(340, 330),
                Text = "Дата покупки:",

            };
            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 350),
                Width = 100,
                Name = "nameTextBox"
            };
            typeTextBox = new TextBox
            {
                Location = new System.Drawing.Point(120, 350),
                Width = 100,
                Name = "typeTextBox"
            };
            serialNumberTextBox = new TextBox
            {
                Location = new System.Drawing.Point(230, 350),
                Width = 100,
                Name = "serialNumberTextBox"

            };
            purchaseDatePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(340, 350),
                Name = "purchaseDatePicker"

            };
            lastMaintenanceDatePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(purchaseDatePicker.Right + 10, 350),
                Name = "lastMaintenanceDatePicker"

            };
            lastMaintenanceLabel = new Label
            {
                Location = new System.Drawing.Point(purchaseDatePicker.Right + 10, 330),
                Text = "Дата последнего обслуждживания:",
                AutoSize = true,
                Name = "lastMaintenanceLabel"

            };
            notifications = new Label
            {
                Text = "Уведомлять о необходимости обслуживания каждые: ",
                Location = new System.Drawing.Point(10, nameTextBox.Bottom + 10),
                AutoSize = true,
            };
            notificationsCmb = new ComboBox
            {
                Location = new System.Drawing.Point(15, notifications.Bottom + 5),
                Width = 150, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "notificationsCmb"
            };
            notificationsCmb.Items.AddRange(new object[] { "3 месяца", "6 месяца", "12 месяца" });
            notificationsCmb.SelectedIndex = 1;
            notificationsCmb.SelectedIndexChanged += notificationsCmb_SelectedItem;
            var addButton = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(560, 420),
                Name = "addButton"

            };
            addButton.Click += AddButton_Click;
            var removeButton = new Button
            {
                Text = "Удалить",
                Location = new System.Drawing.Point(560, 450),
                Name = "removeButton"

            };
            removeButton.Click += RemoveButton_Click;
            var displayButton = new Button
            {
                Text = "Отобразить информацию",
                Location = new System.Drawing.Point(560, 480),
                Name = "displayButton"

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
            this.Controls.Add(notifications);
            this.Controls.Add(notificationsCmb);
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
                int interval = 6;
                switch (notificationsCmb.SelectedIndex)
                {
                    case 0: interval = 3; break;
                    case 1: interval = 6; break;
                    case 2: interval = 12; break;
                }

                var equipment = new Equipment(
                    nameTextBox.Text,
                    typeTextBox.Text,
                    serialNumberTextBox.Text,
                    purchaseDatePicker.Value,
                    lastMaintenanceDatePicker.Value,
                    EquipmentStatus.InGoodCondition)
                {
                    MaintenanceIntervalMonths = interval // Устанавливаем интервал только для нового оборудования
                };

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
            equipmentManager.CheckMaintenanceNotifications();
            equipmentManager.DisplayEquipmentInfo();
            Clear();
        }

        private void notificationsCmb_SelectedItem(object sender, EventArgs e)
        {
            int month = 6;
            switch (notificationsCmb.SelectedIndex)
            {
                case 0: month = 3; break;
                case 1: month = 6; break;
                case 2: month = 9; break;
            }

            equipmentManager.SetMaintenanceInterValForAll(month);
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
