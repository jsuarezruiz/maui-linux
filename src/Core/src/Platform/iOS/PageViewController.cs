﻿using Microsoft.Maui.Handlers;
using UIKit;

namespace Microsoft.Maui
{
	public class PageViewController : ContainerViewController
	{
		public PageViewController(IView page, IMauiContext mauiContext)
		{
			CurrentView = page;
			Context = mauiContext;

			LoadFirstView(page);
		}

		protected override UIView CreateNativeView(IElement view)
		{
			return new PageView
			{
				CrossPlatformArrange = ((IView)view).Arrange,
				CrossPlatformMeasure = ((IView)view).Measure
			};
		}

		public override void TraitCollectionDidChange(UITraitCollection? previousTraitCollection)
		{
			if (CurrentView?.Handler is ElementHandler handler)
			{
				var application = handler.GetRequiredService<IApplication>();
				application?.ThemeChanged();
			}

			base.TraitCollectionDidChange(previousTraitCollection);
		}
	}
}