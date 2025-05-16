using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Управление_техникой;

namespace МодульноеТестирование_ЛР2
{
    // тесты для класса Equipment
    [TestClass]
    public class EquipmentTests
    {
        [TestMethod]
        public void Equipment_Constructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            var name = "Equipment_1";
            var type = "Type A";
            var serial = "12345";
            var purchaseDate = new DateTime(2024, 05, 15);
            var maintenanceDate = new DateTime(2024, 05, 15);
            var status = EquipmentStatus.InGoodCondition;

            // Act
            var equipment = new Equipment(name, type, serial, purchaseDate, maintenanceDate, status);

            // Assert
            Assert.AreEqual(name, equipment.Name);
            Assert.AreEqual(type, equipment.Type);
            Assert.AreEqual(serial, equipment.SerialNumber);
            Assert.AreEqual(purchaseDate, equipment.PurchaseDate);
            Assert.AreEqual(maintenanceDate, equipment.LastMaintenanceDate);
            Assert.AreEqual(status, equipment.Status);
        }

        [TestMethod]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 15), new DateTime(2025, 05, 15), EquipmentStatus.InGoodCondition);

            // Act
            var result = equipment.ToString();

            // Assert
            StringAssert.Contains(result, "Оборудование: Printer");
            StringAssert.Contains(result, "Тип: Office");
            StringAssert.Contains(result, "Серийный номер: A00001");
            StringAssert.Contains(result, "Дата покупки: 15.05.2024");
            StringAssert.Contains(result, "Дата последнего обслуживания: 15.05.2025");
            StringAssert.Contains(result, "Состояние: InGoodCondition");
        }

        [TestMethod]
        public void PerformMaintenance_SetsCurrentDate()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 10), new DateTime(2025, 05, 10), EquipmentStatus.Broken);
            var beforeTest = DateTime.Now;

            // Act
            equipment.PerformMaintenance();

            // Assert
            Assert.IsTrue(equipment.LastMaintenanceDate >= beforeTest);
        }

        [TestMethod]
        public void PerformMaintenance_SetsCurrentGoodStatus()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 10), new DateTime(2025, 05, 10), EquipmentStatus.Broken);
            var beforeTest = DateTime.Now;

            // Act
            equipment.PerformMaintenance();

            // Assert
            Assert.AreEqual(EquipmentStatus.InGoodCondition, equipment.Status);
        }

        [TestMethod]
        public void PerformMaintenance_ShowMessage()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 10), new DateTime(2025, 05, 10), EquipmentStatus.Broken);
            var beforeTest = DateTime.Now;

            // Act
            equipment.PerformMaintenance();

            // Assert
            // проверить, появилось ли сообщение о том, что обслуживание заданного оборудования выполнено 
        }

        [TestMethod]
        public void MarkAsBroken_SetsBrokenStatus()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 05), new DateTime(2025, 05, 05), EquipmentStatus.InGoodCondition);

            // Act
            equipment.MarkAsBroken();

            // Assert
            Assert.AreEqual(EquipmentStatus.Broken, equipment.Status);
        }

        [TestMethod]
        public void MarkAsBroken_ShowMessage()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 05, 05), new DateTime(2025, 05, 05), EquipmentStatus.InGoodCondition);

            // Act
            equipment.MarkAsBroken();

            // Assert
            // проверить, появилось ли сообщение о том, что заданное оборудование помечено как неисправное 
        }

        [TestMethod]
        public void MarkAsInRepair_SetsInRepairStatus()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 04, 20), new DateTime(2025, 04, 20), EquipmentStatus.Broken);

            // Act
            equipment.MarkAsInRepair();

            // Assert
            Assert.AreEqual(EquipmentStatus.InRepair, equipment.Status);
        }

        [TestMethod]
        public void MarkAsInRepair_ShowMessage()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", new DateTime(2024, 04, 20), new DateTime(2025, 04, 20), EquipmentStatus.Broken);

            // Act
            equipment.MarkAsInRepair();

            // Assert
            // проверить, появилось ли сообщение о том, что заданное оборудование отправлено на ремонт 
        }
    }

    // тесты для класса EquipmentManager
    [TestClass]
    public class EquipmentManagerTests
    {
        private EquipmentManager _manager;
        private EquipmentManager equipmentManager;
        private ListView _listView;
        private ListView listView;

        [TestInitialize]
        public void Setup()
        {
            _listView = new ListView();
            _manager = new EquipmentManager(_listView);
        }

        [TestMethod]
        public void AddEquipment_WithValidEquipment_AddsToList()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", DateTime.Now, DateTime.Now, EquipmentStatus.InGoodCondition);

            // Act
            _manager.AddEquipment(equipment);

            // Assert
            Assert.AreEqual(1, _listView.Items.Count);
            Assert.AreEqual("Printer", _listView.Items[0].SubItems[0].Text);
        }

        [TestMethod]
        public void AddEquipment_WithNull_DoesNotAddToList()
        {
            // Act
            _manager.AddEquipment(null);

            // Assert
            Assert.AreEqual(0, _listView.Items.Count);
        }

        [TestMethod]
        public void AddEquipment_WithNull_ShowMessage()
        {
            // Act
            _manager.AddEquipment(null);

            // Assert
            // проверить, появилось ли сообщение о том, что объект оборудования не создан
        }

        [TestMethod]
        public void RemoveEquipment_WithExistingSerialNumber_FindsEquipment()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", DateTime.Now, DateTime.Now, EquipmentStatus.InGoodCondition);
            _manager.AddEquipment(equipment);

            // Assert
            StringAssert.Contains(equipment.SerialNumber, "A00001");
        }

        [TestMethod]
        public void RemoveEquipment_WithExistingSerialNumber_RemovesFromList()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", DateTime.Now, DateTime.Now, EquipmentStatus.InGoodCondition);
            _manager.AddEquipment(equipment);

            // Act
            _manager.RemoveEquipment("A00001");

            // Assert
            Assert.AreEqual(0, _listView.Items.Count);
        }

        [TestMethod]
        public void RemoveEquipment_WithNonExistingSerialNumber_DoesNotRemove()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", DateTime.Now, DateTime.Now, EquipmentStatus.InGoodCondition);
            _manager.AddEquipment(equipment);

            // Act
            _manager.RemoveEquipment("0000");

            // Assert
            Assert.AreEqual(1, _listView.Items.Count);
        }

        [TestMethod]
        public void RemoveEquipment_WithNonExistingSerialNumber_ShowMessage()
        {
            // Arrange
            var equipment = new Equipment("Printer", "Office", "A00001", DateTime.Now, DateTime.Now, EquipmentStatus.InGoodCondition);
            _manager.AddEquipment(equipment);

            // Act
            _manager.RemoveEquipment("0000");

            // Assert
            // проверить, появилось ли сообщение о том, что объект оборудования с таким серийным нномером не найден
        }

        [TestMethod]
        public void DisplayEquipmentInfo_EmptyMessage()
        {
            _manager.DisplayEquipmentInfo();

            // Assert
            // проверить, что появилось сообщение о пустом списке
        }

        [TestMethod]
        public void LoadEquipment_WithEmptyList_ShowsEmptyListView()
        {
            // Act вызывается внутри после Add/Remove
            // No equipment added

            // Assert
            Assert.AreEqual(0, _listView.Items.Count);
        }
    }

    // тесты для формы MainForm
    [TestClass]
    public class MainFormTests
    {
        [TestMethod]
        public void MainForm_FormDisplay()
        {
            // Act 
            var form = new MainForm();

            // Assert
            // проверить, что появлилась форма 
        }
    }


    // тесты для класса DisplayInfoForm
    [TestClass]
    public class DisplayInfoFormTests
    {
        [TestMethod]
        public void DisplayInfoForm_FormDisplayEmptyMessage()
        {
            // Act
            var form = new MainForm();
            form.ShowDialog();

            // Assert
            // проверить, что вышло сообщение о том что список оборудования пуст или не задан 
        }
    }
}
