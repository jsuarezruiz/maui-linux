﻿using System;

namespace Microsoft.Maui.Handlers
{
	public partial class PageHandler : ViewHandler<IView, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapTitle(PageHandler handler, IView page)
		{
		}
		public static void MapContent(PageHandler handler, IView page)
		{
		}
	}
}
