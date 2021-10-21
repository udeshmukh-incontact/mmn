using System;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Person
    {
        public Guid? Id { get; set; }
        public string TargetName { get; set; }        
        public bool ExternallyOwned { get; set; }        
        public Links Links { get; set; }        
        public string FirstName { get; set; }        
        public string LastName { get; set; }       
        public string Timezone { get; set; }       
        public string ExternalKey { get; set; }
        public List<string> roles { get; set; }
        public string Status { get; set; }        
        public string RecipientType { get; set; }
        public string Language { get; set; }
    }
}
