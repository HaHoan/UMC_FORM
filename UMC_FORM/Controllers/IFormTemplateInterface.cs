using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UMC_FORM.Controllers
{
    public interface IFormTemplateInterface
    {
        JsonResult Accept(object ticket);
        JsonResult Reject(object ticket);
        JsonResult Delete(object ticket);
        JsonResult Create(object ticket);
    }
}