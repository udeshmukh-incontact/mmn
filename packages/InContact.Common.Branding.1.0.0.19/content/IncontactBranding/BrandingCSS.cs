using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace InContact.Common.Branding
{
    public class BrandingCSS
    {
        private readonly IPartnerBrandingServiceHelper _PartnerBrandingServiceHelper;

        public BrandingCSS(IPartnerBrandingServiceHelper partnerBrandingServiceHelper)
        {
            _PartnerBrandingServiceHelper = partnerBrandingServiceHelper;
        }

        public string GetBranndingCSSByAccountNumber(string partnerid)
        {
            string url = "branding/css/accountnumber?account=" + partnerid;
            return _PartnerBrandingServiceHelper.CallApi<string, string>(url, HttpMethod.Get);
        }

        public string GetBranndingCSSByAdfsGuid(string adfsGuid)
        {
            string url = "branding/css/adfsguid?adfsGuid=" + adfsGuid;
            return _PartnerBrandingServiceHelper.CallApi<string, string>(url, HttpMethod.Get);
        }

        public BrandedFooter GetFooter(string partnerid)
        {
            BrandedFooter brandedfooter = new BrandedFooter();

            string url = "branding/getfooter?account=" + partnerid;
            brandedfooter = _PartnerBrandingServiceHelper.CallApi<string, BrandedFooter>(url, HttpMethod.Get);
            return brandedfooter;
        }
    }
}