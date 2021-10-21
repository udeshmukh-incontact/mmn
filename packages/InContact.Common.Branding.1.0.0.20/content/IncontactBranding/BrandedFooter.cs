using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InContact.Common.Branding
{
    public class BrandedFooter
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccountNo { get; set; }
        public string Title { get; set; }
        public List<TopFooterRow> TopFooterRow { get; set; }
        public BottomFooterRow BottomFooterRow { get; set; }
    }

    public class TopFooterRow
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
    }
    public class BottomFooterRow
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}