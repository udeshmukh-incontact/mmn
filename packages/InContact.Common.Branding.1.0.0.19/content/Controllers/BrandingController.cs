
using System.Web.Mvc;
using System.IO;
using System.Linq;
using System;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace InContact.Common.Branding
{
    public class BrandingController : Controller
    {

        private readonly IPartnerBrandingServiceHelper _apiHelper;
        const string _folderLocation = "~/IncontactBranding/PartnerBranding";
        const int _maxDaysOld = 1;
        public BrandingController()
        {
            _apiHelper = new PartnerBrandingServiceHelper(new System.Net.Http.HttpClient());
        }
        public ContentResult GetCssByAccountNumber(string partner)
        {

            string cssContent = "";
            cssContent = GetLatestCssFromCurrentProject(partner, _maxDaysOld);
            if (string.IsNullOrWhiteSpace(cssContent))
            {
                cssContent = GetLatestCssByAccountFromServer(partner);
                SaveCSSFile(partner, cssContent);
            }
            return Content(cssContent, "text/css");
        }


        public ContentResult GetCssByAdfsGuid(string adfsGuid)
        {

            string cssContent = "";
            cssContent = GetLatestCssFromCurrentProject(adfsGuid, _maxDaysOld);
            if (string.IsNullOrWhiteSpace(cssContent))
            {
                cssContent = GetLatestCssByAdfsGuidFromServer(adfsGuid);
                SaveCSSFile(adfsGuid, cssContent);
            }
            return Content(cssContent, "text/css");
        }

        public BrandedFooter GetFooter(string partnerid)
        {
            BrandedFooter brandedfooter = new BrandedFooter();
            brandedfooter = GetLatestFooterFromCurrentProject(partnerid, _maxDaysOld);
            if (string.IsNullOrEmpty(brandedfooter.Key))
            {
                if (!String.IsNullOrEmpty(partnerid))
                {
                    string url = "branding/getfooter?account=" + partnerid;
                    brandedfooter = new BrandingCSS(_apiHelper).GetFooter(partnerid);
                    SaveJSONFile(partnerid, brandedfooter);
                }
            }
            return brandedfooter;
        }


        private string GetLatestCssFromCurrentProject(string partner, int maxDaysOld)
        {
            string directory = _folderLocation+'/'+partner+'/'+"css";
            string path = System.Web.HttpContext.Current.Server.MapPath(directory);
            string cssContent = "";
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo file = di.GetFiles("*.css")
                    .Where(p => p.Extension == ".css" && (DateTime.Now - p.CreationTime).TotalDays < maxDaysOld)
                    .OrderByDescending(p => p.CreationTime).FirstOrDefault();
                if (file != null)
                {
                    
                    cssContent =System.IO.File.ReadAllText(file.FullName);
                }
            }
            return cssContent;
        }

        private void SaveCSSFile(string partner, string cssContent)
        {
            if (!string.IsNullOrWhiteSpace(cssContent))
            {
                string directory = _folderLocation+'/'+partner+'/'+"css";
                string fileName = DateTime.Now.ToString("yyyy-MM-d");
                string path = System.Web.HttpContext.Current.Server.MapPath(directory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                System.IO.File.WriteAllText(path+'/'+fileName+".css", cssContent);
            }
        }

        private string GetLatestCssByAccountFromServer(string partner)
        {
            string partnercss = string.Empty;
            partnercss = new BrandingCSS(_apiHelper).GetBranndingCSSByAccountNumber(partner);
            return partnercss;
        }

        private string GetLatestCssByAdfsGuidFromServer(string adfsGuid)
        {
            string partnercss = string.Empty;
            partnercss = new BrandingCSS(_apiHelper).GetBranndingCSSByAdfsGuid(adfsGuid);
            return partnercss;
        }
        private void SaveJSONFile(string partner, BrandedFooter footerContent)
        {
            if (footerContent != null)
            {
                string directory = _folderLocation+'/'+partner+'/'+"footer";
                string fileName = DateTime.Now.ToString("yyyy-MM-d");
                string path = System.Web.HttpContext.Current.Server.MapPath(directory);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var footerjson = new JavaScriptSerializer().Serialize(footerContent);

                System.IO.File.WriteAllText(path+'/'+fileName+".json", footerjson);
            }
        }

        private BrandedFooter GetLatestFooterFromCurrentProject(string partner, int maxDaysOld)
        {
            BrandedFooter brandedfooter = new BrandedFooter();
            string directory = _folderLocation+'/'+partner+'/'+"footer";
            string path = System.Web.HttpContext.Current.Server.MapPath(directory);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo file = di.GetFiles("*.json")
                    .Where(p => p.Extension == ".json" && (DateTime.Now - p.CreationTime).TotalDays < maxDaysOld)
                    .OrderByDescending(p => p.CreationTime).FirstOrDefault();
                if (file != null)
                {
                    var jsondata = System.IO.File.ReadAllText(file.FullName);
                    if (!string.IsNullOrEmpty(jsondata))
                        brandedfooter = JsonConvert.DeserializeObject<BrandedFooter>(jsondata);
                }
            }
            return brandedfooter;
        }
    }
}