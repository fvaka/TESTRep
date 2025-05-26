using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Управление_техникой
{
    public partial class DisplayInfoForm : Form
    {
        public List<Equipment> Equipment { get; set; }
        private TextBox numberTextBox;
        private RichTextBox infoRichTextBox;
        public DisplayInfoForm(List<Equipment> equipment)
        {
            InitializeComponents();
            if (equipment != null && equipment.Count > 0)
            {
                foreach (var equip in equipment)
                {
                    infoRichTextBox.AppendText(equip.ToString() + Environment.NewLine +
                    Environment.NewLine);
                }
            }
            else
            {
                infoRichTextBox.Text = "Список оборудования пуст или не задан.";
            }


        }

        private void InitializeComponents()
        {
            this.Text = "Отобразить информацию";
            this.Width = 795;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;
            infoRichTextBox = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(760, 270),
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
            };
            var numberLabel = new Label
            {
                Text = "Введити серийный номер оборудования для изменения:",
                Location = new Point(10, infoRichTextBox.Bottom + 10),
                AutoSize = true,
            };
            numberTextBox = new TextBox
            {
                Location = new Point(numberLabel.Right + 200, infoRichTextBox.Bottom + 10),
            };
            var GoodCondition = new Button
            {
                Location = new Point(10, infoRichTextBox.Bottom + 40),
                Text = "Выполнить обслуживание"
            };
            GoodCondition.Click += GoodCondition_Click;

            var Broken = new Button
            {
                AutoSize = true,
                Location = new Point(GoodCondition.Right + 10, infoRichTextBox.Bottom + 40),
                Text = "Пометить как неисправное"
            };
            var InRepair = new Button
            {
                AutoSize = true,
                Location = new Point(Broken.Right + 90, infoRichTextBox.Bottom + 40),
                Text = "Отправить на ремонт"
            };

            this.Controls.Add(infoRichTextBox);
            this.Controls.Add(numberTextBox);
            this.Controls.Add(numberLabel);
            this.Controls.Add(GoodCondition);
            this.Controls.Add(Broken);
            this.Controls.Add(InRepair);
        }
        private void GoodCondition_Click(object sender, EventArgs e)
        {
            string serialNum = numberTextBox.Text.Trim(); 
            Equipment.Find(x => x.Equals(serialNum)); // возникает исключение, исправить 
        }

        //private void infoRichTextBox_SelectedChecnged(object sender, EventArgs e)
        //{
        //    Equipment.PerformMaintenance();
        //}
    }
}
