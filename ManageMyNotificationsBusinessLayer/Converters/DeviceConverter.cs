using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsBusinessLayer.Data
{
    [ExcludeFromCodeCoverage]
    public static class DeviceConverter
    {

        public static NotificationContact ToViewModel(this SFContact domain)
        {
            if (domain == null)
                return null;
            var result = new NotificationContact
            {
                AccountId = domain.AccountId,
                AccountNumber = domain.AccountNumber,
                AdfsGuid = domain.AdfsGuid,
                ContactRole = domain.ContactRole,
                CreatedDate = domain.CreatedDate,
                Deactivated = domain.Deactivated,
                Email = domain.Email,
                FirstName = domain.FirstName,
                LastName = domain.LastName,
                PartnerContactId = domain.PartnerContactId,
                UserName = domain.UserName,
                PhoneNumber = domain.Phone,
                XMPersonGuid = domain.XmGuid,
                Id = domain.Id,
                XMPersonId = domain.XmId,
                XMPersonName = domain.XmName,
                AccountRootParentIDs = domain.AccountRootParentIDs!=null? domain.AccountRootParentIDs.ToArray():null,
            };
            return result;
        }
    }
}


