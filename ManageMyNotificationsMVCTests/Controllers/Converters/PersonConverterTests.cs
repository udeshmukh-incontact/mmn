using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Controllers.Converters;
using ManageMyNotificationsMVC.Models;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVCTests.Controllers.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class PersonConverterTests
    {
        [Test]
        public void PersonConverter_ToViewModel_ConvertsProperly()
        {

            var personResponse = new GetPersonResponse()
            {
                FirstName = "PersonFirst",
                LastName = "PersonLast",
                Id = "Id1",
                Timezone = "UASPT",
                TargetName = "targetName1"
            };

            var expectedPerson = new Persons()
            {
                FirstName = "PersonFirst",
                LastName = "PersonLast",
                Id = "Id1",
                TimeZone = "UASPT",
                TargetName = "targetName1"
            };

            var actualPerson = personResponse.ToViewModel();
            Assert.That(Utilities.AreObjectsEquivalent(expectedPerson, actualPerson));
        }
        
        [Test]
        public void PersonConverter_ToViewModel_ConvertsProperly_Null()
        {
            GetPersonResponse personResponse = null;
            var actualPerson = personResponse.ToViewModel();
            Assert.IsNull(actualPerson);
        }

        [Test]
        public void PersonConverter_ToDomainModel_ConvertsProperly()
        {
            var person = new Persons()
            {
                FirstName = "PersonFirst",
                LastName = "PersonLast",
                Id = "Id1",
                TimeZone = "UASPT"
            };

            var expected = new GetPersonResponse()
            {
                FirstName = "PersonFirst",
                LastName = "PersonLast",
                Id = "Id1",
                Timezone = "UASPT"
            };

            var actual = person.ToDomainModel();
            Assert.That(Utilities.AreObjectsEquivalent(expected, actual));
        }

        [Test]
        public void PersonConverter_ToDomainModel_ConvertsProperly_Null()
        {
            Persons person = null;
            var actual = person.ToDomainModel();
            Assert.IsNull(actual);
        }
    }
}
