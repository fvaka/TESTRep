using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Управление_техникой
{
    public partial class DisplayInfoForm : Form
    {
        public List<Equipment> Equipment { get; set; }
        public DisplayInfoForm()
        {
            this.Text = "Отобразить информацию";
            this.Width = 400;
            this.Height = 300;
            var infoTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(380, 270),
                Multiline = true,
                ScrollBars = ScrollBars.Both
            };
            if (Equipment != null && Equipment.Count > 0)
            {
                foreach (var equip in Equipment)
                {
                    infoTextBox.AppendText(equip.ToString() + Environment.NewLine +
                    Environment.NewLine);
                }
            }
            else
            {
                infoTextBox.Text = "Список оборудования пуст или не задан.";
            }
            this.Controls.Add(infoTextBox);
        }
    }
}
