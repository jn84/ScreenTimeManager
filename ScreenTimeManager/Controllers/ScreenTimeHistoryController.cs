using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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
        public ActionResult Index(int? dateId)
        {
			// If dateId is null, give back today's date

	        if (dateId == null)
	        {
		        var h = db.HistoryDates.FirstOrDefault(hd => hd.EntriesDate == DateTime.Today);
		        Debug.WriteLine(h != null ? "Got today's entries: OK" : "Got today's entries: NOT OK");

		        return View(h.EntriesForThisDate.ToList());
	        }

            return View(db.TimeChanged.ToList());
        }

        // GET: ScreenTimeHistory/Details/5
        public ActionResult Details(int? id)
        {
			var ctx = new ScreenTimeManagerContext();

	        return PartialView("_Details", ctx.TimeChanged.Find(id));
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
