using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;
using ScreenTimeManager.Utility;

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

	    public ActionResult ApplyTime(int? id)
	    {
		    var rule = db.Rules.Find(id);

			if (rule == null)
				throw new ArgumentException("The rule ID does not correspond to a rule found in the database." +
				                            " You shouldn't be able to get here if the rule doesn't exist.");
			// We'll need the rule data to build the view
			ViewBag.Rule = rule;

		    TimeSubmission ts = new TimeSubmission()
		    {
			    Hours = 0,
			    Minutes = 0,
				RuleBaseId = rule.Id
		    };

		    if (rule.RuleType == RuleType.Fixed)
			    return PartialView("_FixedInputModal", ts);
			if (rule.RuleType == RuleType.Variable)
				return PartialView("_VariableInputModal", ts);

			throw new ArgumentException("The rule's RuleType (" + rule.RuleType + ") is not valid in this context. " +
			                            "RuleType must be Fixed or Variable. You shouldn't be here. Some other code" +
			                            " did something bad. Good luck!");
	    }

		[HttpPost]
		[ValidateAntiForgeryToken]
	    public ActionResult ApplyTime([Bind(Include = "RuleBaseId, Hours, Minutes")] TimeSubmission timeSubmission)
	    {
		    RuleBase rule = db.Rules.Find(timeSubmission.RuleBaseId);

			if (rule == null)
				throw new ArgumentException("The rule ID does not correspond to a rule found in the database." +
				                            " You shouldn't be able to get here if the rule doesn't exist.");

			if (ModelState.IsValid)
			{
				long? tempMilliseconds =
					TotalScreenTimeManager.
					ConvertHoursMinutesToMilliseconds(timeSubmission.Hours, timeSubmission.Minutes);

				var tstc = TotalScreenTimeManager.GenerateTotalScreenTimeChanged(rule, tempMilliseconds);

				TotalScreenTimeManager.AddOrUpdateRuleAppliedEntry(tstc);

				return Json(new { success = ModelState.IsValid, redirectUrl = Url.Action("Index") });
			}

		    ViewBag.Rule = rule;

			return PartialView(rule.RuleType == RuleType.Fixed ? "_FixedInputModal" : "_VariableInputModal", timeSubmission);
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
