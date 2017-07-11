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
    public class RuleBasesController : Controller
    {
        private ScreenTimeManagerContext db = new ScreenTimeManagerContext();

        // GET: RuleBases
        public ActionResult Index()
        {
            return View(db.Rules.ToList());
        }

	    [HttpPost]
	    [ValidateAntiForgeryToken]
	    public ActionResult FinalizeChangeTime(TotalScreenTimeChanged timeToAdd)
	    {

			
		    return View("Index");
			// TODO: Placeholder. We would rather update the page with a message without reloading it. The timer would be updated dynamically (SignalR) just as it is with keeping synchronized
	    }


	    [HttpPost]
	    [ValidateAntiForgeryToken]
	    public ActionResult SubmitChangeTime(RuleBase rule)
	    {
			// Generate a TotalScreenTimeChanged object

		    return PartialView("_ConfirmApplyRuleOverlay", null);
	    }

		// For Variable Rule
		[HttpPost]
	    [ValidateAntiForgeryToken]
	    public ActionResult SubmitChangeTime(RuleBase rule, string hoursApplied, string minutesApplied)
	    {
		    // Generate a TotalScreenTimeChanged object

			return PartialView("_ConfirmApplyRuleOverlay", null);
		}

        // GET: RuleBases/Details/5
        public ActionResult Details(int? id)
        {

			if (id == null)
				throw new Exception("Passed rule id was null");

	        var timeHistory = new TotalScreenTimeChanged();

			using (var ctx = new ScreenTimeManagerContext())
	        {
				var rule = ctx.Rules.Find(id);

				timeHistory.RecordAddedDateTime = DateTime.Now;

				// TODO: need some error management
		        timeHistory.RuleUsedId = rule.Id;
	        }

			return PartialView("_ConfirmApplyRuleOverlay", timeHistory);
        }

        // GET: RuleBases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RuleBases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MyRuleType,RuleTitle,RuleDescription")] RuleBase ruleBase)
        {
            if (ModelState.IsValid)
            {
                db.Rules.Add(ruleBase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ruleBase);
        }

        // GET: RuleBases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RuleBase ruleBase = db.Rules.Find(id);
            if (ruleBase == null)
            {
                return HttpNotFound();
            }
            return View(ruleBase);
        }

        // POST: RuleBases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MyRuleType,RuleTitle,RuleDescription")] RuleBase ruleBase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ruleBase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ruleBase);
        }

        // GET: RuleBases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RuleBase ruleBase = db.Rules.Find(id);
            if (ruleBase == null)
            {
                return HttpNotFound();
            }
            return View(ruleBase);
        }

        // POST: RuleBases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RuleBase ruleBase = db.Rules.Find(id);
            db.Rules.Remove(ruleBase);
            db.SaveChanges();
            return RedirectToAction("Index");
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
