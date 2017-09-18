using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.Controllers
{
	public class ScreenTimeHistoryController : Controller
	{
		private readonly ScreenTimeManagerContext db = new ScreenTimeManagerContext();

		// GET: ScreenTimeHistory
		public ActionResult Index(int? dateId)
		{
			// If dateId is null, give back today's date
			int selectedIndex = 0;

			if (!db.HistoryDates.Any())
				return View();

			// Ick. Maybe a way to only grab the three we need?
			// TODO: Rework this. Grabbing the whole list is a waste
			List<TimeHistoryDate> hbList = db.HistoryDates.OrderBy(hb => hb.EntriesDate).ToList();

			if (dateId == null || db.HistoryDates.Find(dateId) == null)
			{
				// Tries to find entries for today
				// If there's no entries yet today, do what? For now, get the last valid date
				TimeHistoryDate h =
					db.HistoryDates.FirstOrDefault(hd => hd.EntriesDate == DateTime.Today) ??
					db.HistoryDates.OrderBy(hd => hd.EntriesDate).AsEnumerable().Last();

				selectedIndex = hbList.IndexOf(h);
			}
			else
			{
				selectedIndex = hbList.FindIndex(hb => hb.Id == dateId);
			}

			// Hmm...
			if (selectedIndex - 1 > -1)
				ViewBag.PreviousDate = hbList[selectedIndex - 1];
			else
				ViewBag.PreviousDate = null;


			ViewBag.SelectedDate = hbList[selectedIndex];

			if (selectedIndex + 1 < hbList.Count)
				ViewBag.NextDate = hbList[selectedIndex + 1];
			else
				ViewBag.NextDate = null;

			return View(hbList[selectedIndex].EntriesForThisDate);
		}

		// GET: ScreenTimeHistory/Details/5
		public ActionResult Details(int? id)
		{
			return PartialView("_Details", db.TimeChanged.Find(id));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}