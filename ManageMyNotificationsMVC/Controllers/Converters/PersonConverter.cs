using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Models;

namespace ManageMyNotificationsMVC.Controllers.Converters
{
    public static class PersonConverter
    {

        public static Persons ToViewModel(this GetPersonResponse domain)
        {
            if (domain == null)
                return null;
            var result = new Persons
            {
                FirstName =domain.FirstName,
                LastName =domain.LastName,
                Id =domain.Id,
                TargetName = domain.TargetName,
                TimeZone = domain.Timezone,
                Status = domain.Status
            };

            return result;
        }

        public static GetPersonResponse ToDomainModel(this Persons viewModel)
        {
            if (viewModel == null)
                return null;

            var domain = new GetPersonResponse
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Id = viewModel.Id,
                Timezone = viewModel.TimeZone

            };
            return domain;
        }
    }
}