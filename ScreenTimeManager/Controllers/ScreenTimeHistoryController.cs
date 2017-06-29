using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.Controllers
{
    public class ScreenTimeHistoryController : Controller
    {
        private ScreenTimeManagerContext db = new ScreenTimeManagerContext();

        // GET: ScreenTimeHistory
        public ActionResult Index()
        {
            return View(db.TimeChanged.ToList());
        }

        // GET: ScreenTimeHistory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TotalScreenTimeChanged totalScreenTimeChanged = db.TimeChanged.Find(id);
            if (totalScreenTimeChanged == null)
            {
                return HttpNotFound();
            }
            return View(totalScreenTimeChanged);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
