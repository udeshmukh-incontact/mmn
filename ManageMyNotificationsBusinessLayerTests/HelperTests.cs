using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ManageMyNotificationsBusinessLayerTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void Helper_AddRange_WithCollections()
        {
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            Dictionary<string, string> dict2 = new Dictionary<string, string>();
            dict1.Add("test1","value1");
            dict1.Add("test2", "value2");
            dict2.Add("test1", "value3");
            dict2.Add("test3", "value4");
            dict1.AddRange(dict2);
            Assert.AreEqual(3, dict1.Count);
        }
        [Test]
        public void Helper_AddRange_Null()
        {
            Dictionary<string, string> dict1 = new Dictionary<string, string>();
            Dictionary<string, string> dict2 = null;
            dict1.Add("test1", "value1");
            dict1.Add("test2", "value2");
            Assert.Throws<ArgumentNullException>(() => dict1.AddRange(dict2));
        }

        [Test]
        public void Helper_MergeRangeToAccount_Null()
        {
            Dictionary<string, XMUserDetails> dict1 = new Dictionary<string, XMUserDetails>();
            Dictionary<string, XMUserDetails> dict2 = null;
            dict1.Add("test1", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100001" } },
            });
            dict1.Add("test2", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100002" } },
            });
            Assert.Throws<ArgumentNullException>(() => Helper.MergeRangeToAccount(dict1, dict2));
        }

        [Test]
        public void Helper_MergeRangeToAccount_WithCollections()
        {
            Dictionary<string, XMUserDetails> dict1 = new Dictionary<string, XMUserDetails>();
            Dictionary<string, XMUserDetails> dict2 = new Dictionary<string, XMUserDetails>();
            dict1.Add("test1", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100001" } },
            });
            dict1.Add("test2", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100002" } },
            });

            dict2.Add("test1", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100003" } },
            });
            dict2.Add("test3", new XMUserDetails()
            {
                accounts = new CadeBillAccount[] { new CadeBillAccount() { CadebillAccountNo__c = "100004" } },
            });

            Helper.MergeRangeToAccount(dict1, dict2);

            Assert.AreEqual(2, dict1["test1"].accounts.Count());
        }
    }
}
