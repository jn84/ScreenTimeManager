using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Controllers
{
	public class RuleBasesController : Controller
	{
		private readonly ScreenTimeManagerContext db = new ScreenTimeManagerContext();

		// GET: RuleBases
		public ActionResult Index()
		{
			RuleBaseViewModel rb = new RuleBaseViewModel
			{
				Rules = db.Rules.Where(r => !r.IsExpired && !r.IsHidden).AsEnumerable(),
				Requests = db.TimeRequests.AsEnumerable()
			};

			return View(rb);
		}


		// This is an AJAX only controller
		// Don't bother with antiforgery since we're not changing the server state
		// (and because I don't know how to use it with AJAX)
		[HttpPost]
		public ActionResult UpdatePendingTime(string formData)
		{
			Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(formData);
			int ruleId;
			int hours, minutes;

			if (!int.TryParse(data["RuleBaseId"], out ruleId) ||
			    !int.TryParse(data["Hours"], out hours) ||
			    !int.TryParse(data["Minutes"], out minutes))
				return Json(new {success = false});

			RuleBase rule = db.Rules.Find(ruleId);

			if (rule == null)
				return Json(new {success = false});

			long? unmodifiedMilliseconds = TotalScreenTimeManager.ConvertHoursMinutesToMilliseconds(hours, minutes);

			if (unmodifiedMilliseconds == null)
				return Json(new {success = false});

			long resultMilliseconds =
				TotalScreenTimeManager.GetModifiedTimeInMilliseconds(
					rule,
					(long) unmodifiedMilliseconds);

			// Who ya gonna call? TotalScreenTimeManager!

			return Json(new
			{
				success = true,
				// Why are we using longs if we'll just willy nilly cast everything to int?
				timespan = TotalScreenTimeManager.FormatTimeSpan(resultMilliseconds),
				modifier = rule.RuleModifier == RuleModifier.Add ? "add" : "subtract",
				redirectUrl = Url.Action("Index")
			});
		}

		[Authorize(Roles = "Admin,Parent")]
		public ActionResult ApplyTime(int? id)
		{
			RuleBase rule = db.Rules.Find(id);

			if (rule == null)
				throw new ArgumentException("The rule ID does not correspond to a rule found in the database." +
				                            " You shouldn't be able to get here if the rule doesn't exist.");

			// We'll need the rule data to build the view
			ViewBag.Rule = rule;

			TimeSubmission ts = TimeSubmission.Create(rule.Id);

			if (rule.RuleType == RuleType.Fixed)
				return PartialView("_FixedInputModal", ts);
			if (rule.RuleType == RuleType.Variable)
				return PartialView("_VariableInputModal", ts);

			throw new ArgumentException("The rule's RuleType (" + rule.RuleType + ") is not valid in this context. " +
			                            "RuleType must be Fixed or Variable. You shouldn't be here. Some other code" +
			                            " did something bad. Good luck!");
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Parent")]
		[ValidateAntiForgeryToken]
		public ActionResult ApplyTime([Bind(Include = "RuleBaseId, Hours, Minutes, Note, User")] TimeSubmission timeSubmission)
		{
			if (ModelState.IsValid)
			{
				var tstc = TotalScreenTimeManager.GenerateTotalScreenTimeChangedApproved(timeSubmission);

				tstc.IsDenied = false;
				tstc.IsFinalized = true;

				TotalScreenTimeManager.AddOrUpdateRuleAppliedEntry(tstc);

				return Json(new {success = ModelState.IsValid, redirectUrl = Url.Action("Index")});
			}

			RuleBase rule =
				db.Rules.Find(timeSubmission.RuleBaseId) ??
				throw new Exception("RuleBase ID was invalid or the rule does not exist");

			ViewBag.Rule = rule;

			return PartialView(rule.RuleType == RuleType.Fixed ? "_FixedInputModal" : "_VariableInputModal", timeSubmission);
		}

		[Authorize(Roles = "Child")]
		public ActionResult RequestTime(int? id)
		{
			RuleBase rule = db.Rules.Find(id);

			if (rule == null)
				throw new ArgumentException("The rule ID does not correspond to a rule found in the database." +
				                            " You shouldn't be able to get here if the rule doesn't exist.");

			// We'll need the rule data to build the view
			ViewBag.Rule = rule;

			TimeSubmission ts = TimeSubmission.Create(rule.Id);

			if (rule.RuleType == RuleType.Fixed)
				return PartialView("_FixedInputModal", ts);
			if (rule.RuleType == RuleType.Variable)
				return PartialView("_VariableInputModal", ts);

			throw new ArgumentException("The rule's RuleType (" + rule.RuleType + ") is not valid in this context. " +
			                            "RuleType must be Fixed or Variable. You shouldn't be here. Some other code" +
			                            " did something bad. Good luck!");
		}

		[HttpPost]
		[Authorize(Roles = "Child")]
		[ValidateAntiForgeryToken]
		public ActionResult RequestTime([Bind(Include = "RuleBaseId, Hours, Minutes, Note, User")] TimeSubmission timeSubmission)
		{
			if (ModelState.IsValid)
			{
				var tstcr = TotalScreenTimeManager.GenerateTotalScreenTimeChangedRequest(timeSubmission);

				TotalScreenTimeManager.AddOrUpdateRuleAppliedRequest(tstcr);

				return Json(new {success = ModelState.IsValid, redirectUrl = Url.Action("Index")});
			}

			RuleBase rule = 
				db.Rules.Find(timeSubmission.RuleBaseId) ?? 
				throw new Exception("RuleBase ID was invalid or the rule does not exist");

			return PartialView(rule.RuleType == RuleType.Fixed ? "_FixedInputModal" : "_VariableInputModal", timeSubmission);
		}

		[Authorize(Roles = "Admin,Parent")]
		public ActionResult ApproveTime(int? id)
		{
			TotalScreenTimeChangedRequest tstcr = db.TimeRequests.Find(id);

			return PartialView("_ApproveDenyRequest", tstcr);
		}


		[HttpPost]
		[Authorize(Roles = "Admin,Parent")]
		[ValidateAntiForgeryToken]
		public ActionResult ApproveTime([Bind(Include = "Id, SecondsAdded, RuleUsedId, RequestNote, ApprovalNote, RequestedBy, IsApproved")] TotalScreenTimeChangedRequest tstcr)
		{
			if (tstcr == null)
				throw new Exception("TotalScreenTimeChangedRequest was null for some reason");

			if (tstcr.IsApproved == null)
				ModelState.AddModelError("IsApproved", @"You must choose to either approve or deny the request");

			if (ModelState.IsValid)
			{
				var tstc = TotalScreenTimeManager.HandleRequest(tstcr, User.Identity.Name);

				tstc.IsFinalized = true;

				TotalScreenTimeManager.AddOrUpdateRuleAppliedEntry(tstc);

				TotalScreenTimeManager.ArchiveRequest(tstcr);

				return Json(new { success = ModelState.IsValid, redirectUrl = Url.Action("Index") });
			}

			// Have to attach and manually load the navigation property since the view needs it.
			// Property will be null if we don't do this.
			db.TimeRequests.Attach(tstcr);
			db.Entry(tstcr).Reference(r => r.Rule).Load();

			return PartialView("_ApproveDenyRequest", tstcr);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}