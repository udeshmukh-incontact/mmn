using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManageMyNotificationsMVC.Models
{
    public class Device
    {
        private TimeFrame _devicetimeframe;
        public string Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }
        public string DeviceType { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }

        public string CountryCode { get; set; }
        public List<TimeFrame> DeviceTimeFrame { get; set; }
        //{
        //    get
        //    {
        //        if ( _devicetimeframe!=null && DeviceType.ToUpper() == "EMAIL")
        //        {
        //            _devicetimeframe.Name = "24x7x365";
        //            _devicetimeframe.StartTime = "00:00";
        //            _devicetimeframe.DurationInMinutes = 1440;
        //            _devicetimeframe.Days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };
        //            _devicetimeframe.ExcludeHolidays = false;
        //        }
        //        return _devicetimeframe;
        //    }

        //    set
        //    {
        //        _devicetimeframe = value;
        //    }
        //}


        public string Status { get; set; }
    }
}