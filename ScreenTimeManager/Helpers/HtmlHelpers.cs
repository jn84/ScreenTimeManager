using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
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
			string controllerName)
		{
			if (historyDate == null)
				return htmlHelper.ActionLink(
					" ",
					null,
					null,
					null,
					new {@class = "history-paged-link-null"});

			return htmlHelper.ActionLink(
				historyDate.EntriesDate == DateTime.Today ? "Today" : historyDate.EntriesDate.ToString("MMM dd, yyyy"),
				actionName, controllerName,
				null,
				null,
				"begin-time-history-data",
				new {dateId = historyDate.Id},
				new {@class = "history-paged-link"});
		}

		public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
		{
			return evaluation ? value : MvcHtmlString.Empty;
		}

		public static MvcHtmlString DisplayTimeSpanAsDateTime<TModel, TValue>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression)
		{
			object o = expression.Compile().Invoke(htmlHelper.ViewData.Model);
			if (o is TimeSpan)
				return new MvcHtmlString(DateTime.Today.Add((TimeSpan) o).ToString("h\\:mm\\:ss tt"));
			return htmlHelper.DisplayFor(expression);
		}
	}
}