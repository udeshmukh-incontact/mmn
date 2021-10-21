using ManageMyNotificationsBusinessLayer.Data;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsBusinessLayerTests.Data
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class NotificationGroupSaveTest
    {
        [Test]
        public void IsAddDevice_Get_True()
        {
            var device = new NotificationGroupSave()
            {
                IsSelectedCurrent = true,
                IsSelectedPrevious = false
            };
            Assert.AreEqual(device.IsAddDevice, true);
        }

        [Test]
        public void IsAddDevice_Get_False()
        {
            var device1 = new NotificationGroupSave()
            {
                IsSelectedCurrent = true,
                IsSelectedPrevious = true
            };
            var device2 = new NotificationGroupSave()
            {
                IsSelectedCurrent = false,
                IsSelectedPrevious = true
            };
            var device3 = new NotificationGroupSave()
            {
                IsSelectedCurrent = false,
                IsSelectedPrevious = false
            };
            Assert.AreEqual(device1.IsAddDevice, false);
            Assert.AreEqual(device2.IsAddDevice, false);
            Assert.AreEqual(device3.IsAddDevice, false);
        }

        [Test]
        public void IsRemoveDevice_Get_True()
        {
            var device = new NotificationGroupSave()
            {
                IsSelectedCurrent = false,
                IsSelectedPrevious = true
            };
            Assert.AreEqual(device.IsRemoveDevice, true);
        }

        [Test]
        public void IsRemoveDevice_Get_False()
        {
            var device1 = new NotificationGroupSave()
            {
                IsSelectedCurrent = true,
                IsSelectedPrevious = true
            };
            var device2 = new NotificationGroupSave()
            {
                IsSelectedCurrent = true,
                IsSelectedPrevious = false
            };
            var device3 = new NotificationGroupSave()
            {
                IsSelectedCurrent = false,
                IsSelectedPrevious = false
            };
            Assert.AreEqual(device1.IsRemoveDevice, false);
            Assert.AreEqual(device2.IsRemoveDevice, false);
            Assert.AreEqual(device3.IsRemoveDevice, false);
        }
    }
}
