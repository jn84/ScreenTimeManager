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
using ScreenTimeManager.Models.Enums;


//
//
//
//
//	Next to do: Implement a controller method (and possibly a universal class in DataModel or Model) to get calculated time samples
//
//
//
//


namespace ScreenTimeManager.Controllers
{
    public class ManageRulesController : Controller
    {
        private ScreenTimeManagerContext db = new ScreenTimeManagerContext();

        // GET: ManageRules
        public ActionResult Index()
        {
            return View(db.Rules.ToList());
        }

        // GET: ManageRules/Create
        public ActionResult Create(int ruleType)
        {
	        if (ruleType == (int)RuleType.Fixed)
		        return PartialView("_CreateFixedRuleModal");
			else
				return PartialView("_CreateVariableRuleModal");
        }

        // POST: ManageRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RuleType,RuleTitle,RuleDescription,FixedTimeEarned,VariableRatioNumerator,VariableRatioDenominator,RuleModifier")] RuleBase ruleBase)
        {
			ValidateRule(ruleBase);

            if (ModelState.IsValid)
            {
                db.Rules.Add(ruleBase);
                db.SaveChanges();
				// The request succeeded, so we'll let the client know this so the modal can be closed and the new page can be loaded
				return Json( new {success = ModelState.IsValid, redirectUrl = Url.Action("Index")} );
            }

	        if (ruleBase.RuleType == RuleType.Fixed)
		        return PartialView("_CreateFixedRuleModal", ruleBase);
			return PartialView("_CreateVariableRuleModal", ruleBase);
        }

        // GET: ManageRules/Edit/5
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

	        if (ruleBase.RuleType == RuleType.Fixed)
		        return PartialView("_EditFixedRuleModal", ruleBase);

			return PartialView("_EditVariableRuleModal", ruleBase);
        }

        // POST: ManageRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RuleType,RuleTitle,RuleDescription,FixedTimeEarned,VariableRatioNumerator,VariableRatioDenominator,RuleModifier")] RuleBase ruleBase)
        {
			ValidateRule(ruleBase);

            if (ModelState.IsValid)
            {
	            var oldRule = db.Rules.Find(ruleBase.Id);
				if (oldRule != null)
					oldRule.IsExpired = true;
				else
					return HttpNotFound();

				db.Rules.Add(ruleBase);
                db.SaveChanges();

				return Json(new { success = ModelState.IsValid, redirectUrl = Url.Action("Index") });
			}

			if (ruleBase.RuleType == RuleType.Fixed)
		        return PartialView("_EditFixedRuleModal", ruleBase);

	        return PartialView("_EditVariableRuleModal", ruleBase);
		}

        // GET: ManageRules/Delete/5
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

            return PartialView("_DeleteRuleModal", ruleBase);
        }

        // POST: ManageRules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
	        RuleBase ruleBase = db.Rules.Find(id);
	        if (ruleBase == null)
	        {
		        return HttpNotFound();
	        }

			ruleBase.IsExpired = true;
            db.SaveChanges();
	        return Json(new { success = true, redirectUrl = Url.Action("Index") });
		}

	    private void ValidateRule(RuleBase rule)
	    {
			if ((int)rule.RuleModifier != -1 && (int)rule.RuleModifier != 1)
			    ModelState.AddModelError("RuleModifier", @"Please select a value");

		    if (rule.FixedTimeEarned >= TimeSpan.FromDays(1))
			    ModelState.AddModelError("FixedTimeEarned", @"Please enter a value less than 1 day (hh:mm:ss)");
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
