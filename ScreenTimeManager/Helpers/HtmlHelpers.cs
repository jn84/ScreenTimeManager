using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ScreenTimeManager.Enums;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.Helpers
{
	public static class HtmlHelpers
	{
		public static MvcHtmlString NavLink(
			this HtmlHelper htmlHelper,
			string linkText,
			string actionName,
			string controllerName
		)
		{
			string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
			string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

			if (actionName == currentAction && controllerName == currentController)
				return htmlHelper.ActionLink(linkText, actionName, controllerName, null, new {@class = "nav-link-active"});

			return htmlHelper.ActionLink(linkText, actionName, controllerName, null, new {@class = "nav-link"});
		}

		public static MvcHtmlString PagedDateActionLink(
			this HtmlHelper htmlHelper,
			TimeHistoryDate historyDate,
			string actionName,
			string controllerName,
			DatePage page)
		{
			if (historyDate == null)
				return htmlHelper.ActionLink(
					" ",
					null,
					null,
					null,
					new {@class = "history-paged-link-null"});

			string buttonClass = "";

			switch (page)
			{
				case DatePage.Previous:
					buttonClass = "history-paged-link date-page-previous";
					break;
				case DatePage.Next:
					buttonClass = "history-paged-link date-page-next";
					break;
				default:
					buttonClass = "history-paged-link date-page-default";
					break;
			}

			return htmlHelper.ActionLink(
				historyDate.EntriesDate == DateTime.Today ? "Today" : historyDate.EntriesDate.ToString("MMM dd, yyyy"),
				actionName, controllerName,
				null,
				null,
				"begin-time-history-data",
				new {dateId = historyDate.Id},
				new {@class = buttonClass});
		}

		public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
		{
			return evaluation ? value : MvcHtmlString.Empty;
		}

		//public static MvcHtmlString DisplayTimeSpanAsDateTime<TModel, TValue>(
		//	this HtmlHelper<TModel> htmlHelper,
		//	Expression<Func<TModel, TValue>> expression)
		//{
		//	object o = expression.Compile().Invoke(htmlHelper.ViewData.Model);
		//	if (o is TimeSpan)
		//		return new MvcHtmlString(DateTime.Today.Add((TimeSpan) o).ToString("h\\:mm\\:ss tt"));
		//	return htmlHelper.DisplayFor(expression);
		//}
	}
}