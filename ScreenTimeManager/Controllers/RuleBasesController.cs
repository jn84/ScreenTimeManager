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

		///// <summary>
		///// For applying rules to the database
		///// </summary>
		///// <param name="id">The ID of the rule being applied</param>
		///// <param name="hoursApplied"></param>
		///// <param name="minutesApplied"></param>
		//[HttpPost]
	 //   [ValidateAntiForgeryToken]
	 //   public void ChangeTime(int? id, string hoursApplied, string minutesApplied)
		//{
		//	RuleBase rule = null;

		//    using (var ctx = new ScreenTimeManagerContext())
		//    {
		//	    if (id == null)
		//			throw new ModelValidationException("Cannot create new time history entry. Rule ID is null");

		//		rule = ctx.Rules.Find(id);
		//		// Just let it throw the exception. Need to ensure this exception is never thrown.

		//	    long? tempMilliseconds = TotalScreenTimeManager.ConvertHoursMinutesToMilliseconds(hoursApplied, minutesApplied);

		//	    //if (tempMilliseconds == null && rule.RuleType != RuleType.Variable)

		//	    var tstc = TotalScreenTimeManager.GenerateTotalScreenTimeChanged(rule, tempMilliseconds);

		//		TotalScreenTimeManager.AddOrUpdateRuleAppliedEntry(tstc);
		//    }

		//    // Shouldn't.. does it?
		//	if (db == null) throw new NullReferenceException();


		//	//////////////////////////////////////// Should just say "I worked!" or "Validation error!"
	 //   }

  //      // GET: RuleBases/Details/5
  //      public ActionResult Details(int? id)
  //      {

		//	if (id == null)
		//		throw new Exception("Passed rule id was null");

	 //       RuleBase rule;

		//	using (var ctx = new ScreenTimeManagerContext())
		//		rule = ctx.Rules.Find(id);

		//	return PartialView("_ConfirmApplyRuleOverlay", rule);
  //      }

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
