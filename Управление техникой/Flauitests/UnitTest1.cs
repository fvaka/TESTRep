using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
//using System.Windows.Forms;

namespace Flauitests
{
    [TestClass]
    public class FLauiTestsClass
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;


        [TestInitialize]
        public void TestInitialize()
        {
            _app = Application.Launch(@"C:\Users\d1387\source\repos\TESTRep\Управление техникой\Управление техникой\bin\Debug\Управление техникой.exe");
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _automation.Dispose();
            _app?.Close();
        }

        [TestMethod]
        public void AddEquipment_WithValidData_ShouldSucceed()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            addButton.Click();

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование успешно добавлено.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("А0001")));
            Assert.IsNotNull(equipmentItem);

            var subItems = equipmentItem.FindAllChildren();
            Assert.IsTrue(subItems.Length >= 4);
            Assert.AreEqual("Оборудование1", subItems[0].Name);
            Assert.AreEqual("тип1", subItems[1].Name);
            Assert.AreEqual("А0001", subItems[2].Name);
            Assert.AreEqual("InGoodCondition", subItems[3].Name);
        }


        [TestMethod]
        public void RemoveEquipment_BySerialNumber_ShouldSucceed()
        {
            // Добавление нового устройства
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var removeButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("removeButton")).AsButton();

            serialNumberTextBox.Text = "А0001";
            removeButton.Click();

            var messageBox1 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText1 = msgText1.Name;
            StringAssert.Contains(mText1, "Оборудование успешно удалено.");

            var okButton1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton1.Click();

            //var messageBox2 = _mainWindow.ModalWindows.FirstOrDefault();
            //var msgText2 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            //string mText2 = msgText1.Name;
            //StringAssert.Contains(mText2, "Список оборудования пуст.");

            //var okButton2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            //okButton2.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void DisplayEquipmentList_ShouldShowInListView()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 21), new DateTime(2025, 5, 21));

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("А0001")));
            Assert.IsNotNull(equipmentItem);

            var subItems = equipmentItem.FindAllChildren();
            Assert.IsTrue(subItems.Length >= 4);
            Assert.AreEqual("Оборудование1", subItems[0].Name);
            Assert.AreEqual("тип1", subItems[1].Name);
            Assert.AreEqual("А0001", subItems[2].Name);
            Assert.AreEqual("InGoodCondition", subItems[3].Name);
        }

        [TestMethod]
        public void ViewDetailedEquipmentInfo_ShouldDisplayCorrectly()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();
            displayButton.Click();

            var infoWindow = _mainWindow.ModalWindows.FirstOrDefault();
            Assert.IsNotNull(infoWindow);

            var infoTextBox = infoWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(infoTextBox);

            var expectedEquipmentString = $"Оборудование: Оборудование1; Тип: тип1; Серийный номер: А0001;Дата покупки: 19.05.2025; Дата последнего обслуживания: 19.05.2025; Состояние: InGoodCondition\r\n\r\n";
            Assert.IsTrue(infoTextBox.Text.Contains(expectedEquipmentString));
            Assert.IsTrue(infoTextBox.Text.Contains("Оборудование1"));
            Assert.IsTrue(infoTextBox.Text.Contains("А0001"));
            Assert.IsFalse(infoTextBox.Text.Contains("Список оборудования пуст"));

            infoWindow.Close();
        }

        [TestMethod]
        public void MarkAsBroken_ShouldUpdateStatus()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();
            displayButton.Click();

            var infoWindow = _mainWindow.ModalWindows.FirstOrDefault();
            var infoTextBox = infoWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(infoTextBox);

            var numberTextBox = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("numberTextBox")).AsTextBox();
            var brokenButton = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("Broken")).AsButton();

            numberTextBox.Text = "А0001";
            brokenButton.Click();

            var messageBox = infoWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование 'Оборудование1' помечено как неисправное.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var expectedEquipmentString = $"Оборудование: Оборудование1; Тип: тип1; Серийный номер: А0001;Дата покупки: 19.05.2025; Дата последнего обслуживания: 19.05.2025; Состояние: Broken\r\n\r\n";
            Assert.IsTrue(infoTextBox.Text.Contains(expectedEquipmentString));
            Assert.IsTrue(infoTextBox.Text.Contains("Broken"));
            infoWindow.Close();
        }

        [TestMethod]
        public void PerformMaintenance_ShouldUpdateStatus()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();
            displayButton.Click();

            var infoWindow = _mainWindow.ModalWindows.FirstOrDefault();
            var infoTextBox = infoWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(infoTextBox);

            // сначала делаем оборудование неисправным
            var numberTextBox = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("numberTextBox")).AsTextBox();
            var brokenButton = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("Broken")).AsButton();

            numberTextBox.Text = "А0001";
            brokenButton.Click();
            var messageBox = infoWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование 'Оборудование1' помечено как неисправное.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();
            Assert.IsTrue(infoTextBox.Text.Contains("Broken"));

            var numberTextBox1 = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("numberTextBox")).AsTextBox();
            var maintenanceButton = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("GoodCondition")).AsButton();

            numberTextBox.Text = "А0001";
            maintenanceButton.Click();

            var messageBox2 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText2 = msgText2.Name;
            StringAssert.Contains(mText2, "Обслуживание оборудования 'Оборудование1' выполнено.");

            var okButton2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton2.Click();

            var expectedEquipmentString = $"Оборудование: Оборудование1; Тип: тип1; Серийный номер: А0001;Дата покупки: 19.05.2025; Дата последнего обслуживания: 13.06.2025; Состояние: InGoodCondition\r\n\r\n";
            Assert.IsTrue(infoTextBox.Text.Contains(expectedEquipmentString));
            Assert.IsTrue(infoTextBox.Text.Contains("InGoodCondition"));
            infoWindow.Close();
        }

        [TestMethod]
        public void MarkAsInRepair_ShouldUpdateStatus()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();
            displayButton.Click();

            var infoWindow = _mainWindow.ModalWindows.FirstOrDefault();
            var infoTextBox = infoWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(infoTextBox);

            var numberTextBox = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("numberTextBox")).AsTextBox();
            var repairButton = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("InRepair")).AsButton();

            numberTextBox.Text = "А0001";
            repairButton.Click();

            var messageBox = infoWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование 'Оборудование1' отправлено в ремонт.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var expectedEquipmentString = $"Оборудование: Оборудование1; Тип: тип1; Серийный номер: А0001;Дата покупки: 19.05.2025; Дата последнего обслуживания: 19.05.2025; Состояние: InRepair\r\n\r\n";
            Assert.IsTrue(infoTextBox.Text.Contains(expectedEquipmentString));
            Assert.IsTrue(infoTextBox.Text.Contains("InRepair"));
            infoWindow.Close();
        }

        [TestMethod]
        public void AddEquipment_WithEmptyFields_ShouldShowError()
        {
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Заполните все обязательные поля.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 0;
            Assert.IsTrue(equipmentItem);
        }

        [TestMethod]
        public void AddEquipment_WithInvalidMaintenanceDate_ShouldShowError()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2025, 5, 18);
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Дата покупки не может быть позже даты последнего обслуживания.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();


            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 0;
            Assert.IsTrue(equipmentItem);
        }

        [TestMethod]
        public void AddEquipment_DuplicateSerialNumber_ShouldShowError()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование с таким серийным номером уже существует.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 1;
            Assert.IsNotNull(equipmentItem);
        }

        [TestMethod]
        public void RemoveEquipment_NonExistentSerialNumber_ShouldShowError()
        {
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var removeButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("removeButton")).AsButton();

            serialNumberTextBox.Text = "А0002";
            removeButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование с таким серийным номером не найдено.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click()
                ;
            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 0;
            Assert.IsTrue(equipmentItem);
        }

        [TestMethod]
        public void RemoveEquipment_WithoutSerialNumber_ShouldShowError()
        {
            var removeButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("removeButton")).AsButton();
            removeButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Введите серийный номер для удаления.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click()
                ;
            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 0;
            Assert.IsTrue(equipmentItem);
        }

        private void AddEquipment(string name, string type, string serialNumber, DateTime purchaseDate, DateTime maintenanceDate)
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = name;
            typeTextBox.Text = type;
            serialNumberTextBox.Text = serialNumber;
            purchaseDatePicker.SelectedDate = purchaseDate;
            lastMaintenanceDatePicker.SelectedDate = maintenanceDate;
            addButton.Click();

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();
        }

        [TestMethod]
        public void SucceedMessageAfterAdding()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2025, 5, 19);
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование успешно добавлено.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("А0001")));
            Assert.IsNotNull(equipmentItem);

            var subItems = equipmentItem.FindAllChildren();
            Assert.IsTrue(subItems.Length >= 4);
            Assert.AreEqual("Оборудование1", subItems[0].Name);
            Assert.AreEqual("тип1", subItems[1].Name);
            Assert.AreEqual("А0001", subItems[2].Name);
            Assert.AreEqual("InGoodCondition", subItems[3].Name);
        }

        [TestMethod]
        public void SuccedMessageAfterDeleting()
        {
            // Добавление нового устройства
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var removeButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("removeButton")).AsButton();

            serialNumberTextBox.Text = "А0001";
            removeButton.Click();

            var messageBox1 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText1 = msgText1.Name;
            StringAssert.Contains(mText1, "Оборудование успешно удалено.");

            var okButton1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton1.Click();

            //var messageBox2 = _mainWindow.ModalWindows.FirstOrDefault();
            //var msgText2 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            //string mText2 = msgText1.Name;
            //StringAssert.Contains(mText2, "Список оборудования пуст.");

            //var okButton2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            //okButton2.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void InRepairMessageAfter()
        {
            AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2025, 5, 19), new DateTime(2025, 5, 19));

            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();
            displayButton.Click();

            var infoWindow = _mainWindow.ModalWindows.FirstOrDefault();
            var infoTextBox = infoWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(infoTextBox);

            var numberTextBox = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("numberTextBox")).AsTextBox();
            var repairButton = infoWindow.FindFirstDescendant(cf => cf.ByAutomationId("InRepair")).AsButton();

            numberTextBox.Text = "А0001";
            repairButton.Click();

            var messageBox = infoWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, "Оборудование 'Оборудование1' отправлено в ремонт.");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var expectedEquipmentString = $"Оборудование: Оборудование1; Тип: тип1; Серийный номер: А0001;Дата покупки: 19.05.2025; Дата последнего обслуживания: 19.05.2025; Состояние: InRepair\r\n\r\n";
            Assert.IsTrue(infoTextBox.Text.Contains(expectedEquipmentString));
            Assert.IsTrue(infoTextBox.Text.Contains("InRepair"));
            infoWindow.Close();
        }

        [TestMethod]
        public void MessageAboutExpiredEquipmentAfterAdding()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var intervalTimeCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsComboBox();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2024, 6, 9);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2024, 11, 9);
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, $"Внимание! Добавляемое оборудование 'Оборудование1' (№А0001) имеет просроченное ТО и нуждается в обсуживании\n" +
                $"Последнее обслуживание: 09.11.2024");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 0;
            Assert.IsTrue(equipmentItem);
        }

        [TestMethod]
        public void MessageAboutExpiredEquipmentAfterShowingDetailInfo()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var intervalTimeCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsComboBox();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();
            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2024, 6, 9);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2024, 11, 9);
            addButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, $"Внимание! Добавляемое оборудование 'Оборудование1' (№А0001) имеет просроченное ТО и нуждается в обсуживании\n" +
                $"Последнее обслуживание: 09.11.2024");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var messageBox1 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText1 = msgText1.Name;
            StringAssert.Contains(mText1, "Оборудование успешно добавлено.");

            var okButton1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton1.Click();

            displayButton.Click();

            var messageBox2 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText2 = msgText2.Name;
            StringAssert.Contains(mText2, $"=== ВНИМАНИЕ: Оборудование с просроченным обслуживанием ===\r\n- Оборудование1 (№А0001)\r\n\r\n");

            var okButton2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton2.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 1;
            Assert.IsTrue(equipmentItem);
        }
        [TestMethod]
        public void MessageAboutExpiredEquipmentAfterShowingDetailInfoForSeveralEquipment()
        {
            //AddEquipment("Оборудование1", "тип1", "А0001", new DateTime(2024, 6, 9), new DateTime(2024, 11, 9));
            //AddEquipment("Оборудование2", "тип2", "А0002", new DateTime(2024, 6, 9), new DateTime(2024, 10, 11));

            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var intervalTimeCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsComboBox();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();
            var displayButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2024, 6, 9);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2024, 11, 9);
            addButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, $"Внимание! Добавляемое оборудование 'Оборудование1' (№А0001) имеет просроченное ТО и нуждается в обсуживании\n" +
                $"Последнее обслуживание: 09.11.2024");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var messageBox1 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText1 = msgText1.Name;
            StringAssert.Contains(mText1, "Оборудование успешно добавлено.");

            var okButton1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton1.Click();
            
            var nameTextBox2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var intervalTimeCombo2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsComboBox();
            var addButton2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();
            var displayButton2 = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("displayButton")).AsButton();

            nameTextBox2.Text = "Оборудование2";
            typeTextBox2.Text = "тип2";
            serialNumberTextBox2.Text = "А0002";
            purchaseDatePicker2.SelectedDate = new DateTime(2024, 6, 9);
            lastMaintenanceDatePicker2.SelectedDate = new DateTime(2024, 10, 11);
            addButton.Click();
            var messageBox2 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText2 = msgText2.Name;
            StringAssert.Contains(mText2, $"Внимание! Добавляемое оборудование 'Оборудование2' (№А0002) имеет просроченное ТО и нуждается в обсуживании\n" +
                $"Последнее обслуживание: 11.10.2024");

            var okButton2 = messageBox2.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton2.Click();

            var messageBox3 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText3 = messageBox3.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText3 = msgText3.Name;
            StringAssert.Contains(mText1, "Оборудование успешно добавлено.");

            var okButton3 = messageBox3.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton3.Click();

            displayButton.Click();

            var messageBox4 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText4 = messageBox4.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText4 = msgText4.Name;
            StringAssert.Contains(mText4, $"=== ВНИМАНИЕ: Оборудование с просроченным обслуживанием ===\r\n- Оборудование1 (№А0001)\r\n- Оборудование2 (№А0002)\r\n\r\n");

            var okButton4 = messageBox4.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton4.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 2;
            Assert.IsTrue(equipmentItem);
        }
        [TestMethod]
        public void SetInterval()
        {
            var nameTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("nameTextBox")).AsTextBox();
            var typeTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("typeTextBox")).AsTextBox();
            var serialNumberTextBox = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("serialNumberTextBox")).AsTextBox();
            var purchaseDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("purchaseDatePicker")).AsDateTimePicker();
            var lastMaintenanceDatePicker = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("lastMaintenanceDatePicker")).AsDateTimePicker();
            var intervalTimeCombo = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("notificationsCmb")).AsComboBox();
            var addButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("addButton")).AsButton();

            nameTextBox.Text = "Оборудование1";
            typeTextBox.Text = "тип1";
            serialNumberTextBox.Text = "А0001";
            purchaseDatePicker.SelectedDate = new DateTime(2024, 6, 9);
            lastMaintenanceDatePicker.SelectedDate = new DateTime(2025, 2, 9);
            intervalTimeCombo.Value = "3 месяца";
            addButton.Click();

            // Assert
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText = msgText.Name;
            StringAssert.Contains(mText, $"Внимание! Добавляемое оборудование 'Оборудование1' (№А0001) имеет просроченное ТО и нуждается в обсуживании\n" +
                $"Последнее обслуживание: 09.02.2025");

            var okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton.Click();

            var messageBox1 = _mainWindow.ModalWindows.FirstOrDefault();
            var msgText1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("65535"));
            string mText1 = msgText1.Name;
            StringAssert.Contains(mText1, "Оборудование успешно добавлено.");

            var okButton1 = messageBox1.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
            okButton1.Click();

            var listView = _mainWindow.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var equipmentItem = listView.Items.Length == 1;
            Assert.IsTrue(equipmentItem);
        }
    }
}
