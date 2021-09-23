using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    public class CommentController : Controller
    {

        [HttpPost]
        public JsonResult SaveComments(string ticket, string comments)
        {
            using(var db = new DataContext())
            {
                var sess = Session["user"] as Form_User;
                Form_Comment formcmt = new Form_Comment();
                formcmt.COMMENT_DETAIL = comments;
                formcmt.ID = Guid.NewGuid().ToString();
                formcmt.TICKET = ticket;
                formcmt.COMMENT_USER = sess.NAME;
                formcmt.UPD_DATE = DateTime.Now;
                var cmtExist = db.Form_Comment.Where(r => r.TICKET == ticket).OrderByDescending(h => h.ORDER_HISTORY).FirstOrDefault();
                formcmt.ORDER_HISTORY = cmtExist is null ? 0 : cmtExist.ORDER_HISTORY + 1;
                string msg = "";
                try
                {
                    db.Form_Comment.Add(formcmt);
                    db.SaveChanges();
                    msg = "OK";
                }
                catch (Exception ex)
                {
                    msg = "Err";
                    Debug.WriteLine(ex.Message);
                }



                return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index", "Home");
            }

        }

        [ActionName("GetAllComment")]
        public JsonResult LoadComments(string ticket)
        {
            using(var db = new DataContext())
            {
                var formcmt = db.Form_Comment.Where(r => r.TICKET == ticket).OrderBy(t => t.ORDER_HISTORY).ToList();
                return Json(formcmt, JsonRequestBehavior.AllowGet);
            }
         
        }

    }
}