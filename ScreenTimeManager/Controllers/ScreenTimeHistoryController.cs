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

	        TimeHistoryDate 
				previousDate = null, 
				selectedDate = null, 
				nextDate = null;

	        if (dateId == null || db.HistoryDates.Find(dateId) == null)
	        {
		        // Tries to find entries for today
		        // If there's no entries yet today, do what? For now, get the last valid date
				// The list should never be empty..
		        TimeHistoryDate h =
			        db.HistoryDates.FirstOrDefault(hd => hd.EntriesDate == DateTime.Today) ??
			        db.HistoryDates.OrderBy(hd => hd.EntriesDate).Last();
		        selectedDate = h;

	        }
	        else
	        {
		        selectedDate = db.HistoryDates.Find(dateId);
	        }

			previousDate = db.HistoryDates
		        .OrderBy(thd => thd.EntriesDate)
		        .TakeWhile(thd => thd.EntriesDate != selectedDate.EntriesDate)
				.ToList() // Eww
		        .Last();

			nextDate = db.HistoryDates
				.OrderByDescending(thd => thd.EntriesDate)
				.TakeWhile(thd => thd.EntriesDate != selectedDate.EntriesDate)
				.ToList() // Yuck
				.Last();

			// Hmm...
			ViewData["PreviousDate"] = previousDate;
	        ViewData["SelectedDate"] = selectedDate;
	        ViewData["PreviousDate"] = nextDate;

			// So what if it's null?
			// The view will generate differently if it is.
			// If should never be null unless the TimeHistoryDates table is empty.
			return View("Index", selectedDate?.EntriesForThisDate);
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
