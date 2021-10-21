using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManageMyNotificationsMVC.Models
{
    public class TimeFrame
    {
        private bool _excludeholidays = false;
        [Required(ErrorMessage = "You must provide Timeframe name.")]
        public string Name { get; set; }
        public string StartTime { get; set; }
        public int DurationInMinutes { get; set; }
        public List<string> Days { get; set; }
        public bool ExcludeHolidays
        {
            get { return _excludeholidays; }
            set { _excludeholidays = value; }
        }


        public string EndTime { get; set; }
    }

}