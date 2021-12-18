using System;
using System.Collections.Generic;
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
        public static string ContentVersioned(this UrlHelper self, string contentPath)
        {
            string versionedContentPath = contentPath + "?v=" + Assembly.GetAssembly(typeof(UrlHelperExtensions)).GetName().Version.ToString();
            return self.Content(versionedContentPath);
        }
    }
}
