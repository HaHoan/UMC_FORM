using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace UMC_FORM.Ultils
{
    public static class Extensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
    public static class UrlHelperExtensions
    {
        public static string ContentVersioned(this UrlHelper self, string contentPath,string extension)
        {
            var version = Bet.Util.Config.GetValue("version");
            if(version == "0")
            {
                return self.Content($"{contentPath}.{extension}");
            }
            else
            {
                return self.Content($"{contentPath}[{version}].{extension}");
            }
           
        }

        public static string ConvertViewToString(string viewName, object model, ViewDataDictionary ViewData, ControllerContext ControllerContext)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
    }
}
