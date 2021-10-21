using ManageMyNotificationsBusinessLayer.Config;
using ManageMyNotificationsBusinessLayer.Services.XMatters;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsLogicUnitTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class XMattersServiceHelperTest
    {
        private XMattersConfig _xMattersConfig;
        private XMattersServiceHelper _xMattersServiceHelper;

        [SetUp]
        public void SetUp()
        {
            _xMattersConfig = new XMattersConfig
            {
                Username = "ysoto",
                Password = "Newhire12",
                BaseUrl = "https://incontact-ce-np.na5.xmatters.com/api/xm/1/"
            };
            
        }

        [Test, Property("XMattersServiceHelper", "CallXMatters")]
        public void TestGetXmatterPerson()
        {           
            //TODO: Needs to improve
            //const string url = "people";
            //const string xMatterId = "46c842d6-988e-482a-aa0c-02d2ff37975a";
            //string urlGetSpecificPerson = $"{url}/{xMatterId}";
            //var result = _xMattersServiceHelper.CallXMatters<object, Person>(urlGetSpecificPerson, HttpMethod.Get, null);

            //Assert.IsNotNull(result);
            //Assert.AreEqual(result.Id.ToString(), xMatterId);
        }
    }
}
