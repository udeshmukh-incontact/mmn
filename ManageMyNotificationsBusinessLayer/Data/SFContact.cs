using System;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class SFContact
    {
        public string Id { get; set; }
        public string AdfsGuid { get; set; }
        public string XmGuid { get; set; }
        public string XmId { get; set; }
        public string PartnerContactId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool Deactivated { get; set; }
        public string ContactRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> AccountRootParentIDs { get; set; }
        public string XmName { get; set; }
        public string sfContactId { get; set; }
        public string LanguagePreference { get; set; }
    }
}


