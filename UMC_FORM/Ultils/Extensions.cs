using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Ultils
{
    public static class Extensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
