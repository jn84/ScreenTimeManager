﻿using System.Linq;
using System.Net;
using System.Web.Mvc;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.Controllers
{
	public class ManageRulesController : Controller
	{
		private readonly ScreenTimeManagerContext db = new ScreenTimeManagerContext();

		// GET: ManageRules
		public ActionResult Index()
		{
			return View(db.Rules.Where(r => !r.IsExpired && !r.IsHidden).ToList());
		}

		// GET: ManageRules/Create
		[Authorize(Roles = "Admin,Parent")]
		public ActionResult Create(int ruleType)
		{
			// We give it a rule to build from to fill out the default values

			RuleBase newRule = RuleBase.Create();

			if (ruleType == (int) RuleType.Fixed)
			{
				newRule.RuleType = RuleType.Fixed;
				ViewBag.ModalTitle = "Create New Fixed Rule";
				return PartialView("_FixedRuleModal", newRule);
			}
			newRule.RuleType = RuleType.Variable;
			ViewBag.ModalTitle = "Create New Variable Rule";
			return PartialView("_VariableRuleModal", newRule);
		}

		// POST: ManageRules/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Authorize(Roles = "Admin,Parent")]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include =
				"Id,RuleType,RuleTitle,RuleDescription,FixedTimeEarned,VariableRatioNumerator,VariableRatioDenominator,RuleModifier")]
			RuleBase ruleBase)
		{
			ValidateRule(ruleBase);

			if (ModelState.IsValid)
			{
				db.Rules.Add(ruleBase);
				db.SaveChanges();
				// The request succeeded, so we'll let the client know this so the modal can be closed and the new page can be loaded
				return Json(new {success = ModelState.IsValid, redirectUrl = Url.Action("Index")});
			}

			if (ruleBase.RuleType == RuleType.Fixed)
			{
				ViewBag.ModalTitle = "Create New Fixed Rule";
				return PartialView("_FixedRuleModal", ruleBase);
			}
			ViewBag.ModalTitle = "Create New Variable Rule";
			return PartialView("_VariableRuleModal", ruleBase);
		}

		// GET: ManageRules/Edit/5
		[Authorize(Roles = "Admin,Parent")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			RuleBase ruleBase = db.Rules.Find(id);
			// Exclude the timer rule!
			if (ruleBase == null || ruleBase.RuleType == RuleType.Timer)
				return HttpNotFound();

			if (ruleBase.RuleType == RuleType.Fixed)
			{
				ViewBag.ModalTitle = "Edit Fixed Rule";
				return PartialView("_FixedRuleModal", ruleBase);
			}
			ViewBag.ModalTitle = "Edit Variable Rule";
			return PartialView("_VariableRuleModal", ruleBase);
		}

		// POST: ManageRules/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Authorize(Roles = "Admin,Parent")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(
			[Bind(Include =
				"Id,RuleType,RuleTitle,RuleDescription,FixedTimeEarned,VariableRatioNumerator,VariableRatioDenominator,RuleModifier")]
			RuleBase ruleBase)
		{
			ValidateRule(ruleBase);

			if (ModelState.IsValid)
			{
				RuleBase oldRule = db.Rules.Find(ruleBase.Id);
				if (oldRule != null)
					oldRule.IsExpired = true;
				else
					return HttpNotFound();

				db.Rules.Add(ruleBase);
				db.SaveChanges();

				return Json(new {success = ModelState.IsValid, redirectUrl = Url.Action("Index", "ManageRules")});
			}

			if (ruleBase.RuleType == RuleType.Fixed)
			{
				ViewBag.ModalTitle = "Edit Fixed Rule";
				return PartialView("_FixedRuleModal", ruleBase);
			}
			ViewBag.ModalTitle = "Edit Variable Rule";
			return PartialView("_VariableRuleModal", ruleBase);
		}

		// GET: ManageRules/Delete/5
		[Authorize(Roles = "Admin,Parent")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			RuleBase ruleBase = db.Rules.Find(id);
			if (ruleBase == null)
				return HttpNotFound();

			return PartialView("_DeleteRuleModal", ruleBase);
		}

		// POST: ManageRules/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[Authorize(Roles = "Admin,Parent")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			RuleBase ruleBase = db.Rules.Find(id);
			if (ruleBase == null)
				return HttpNotFound();

			ruleBase.IsExpired = true;
			db.SaveChanges();
			return Json(new {success = true, redirectUrl = Url.Action("Index")});
		}

		private void ValidateRule(RuleBase rule)
		{
			if ((int) rule.RuleModifier != -1 && (int) rule.RuleModifier != 1)
				ModelState.AddModelError("RuleModifier", @"Please select a value");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}