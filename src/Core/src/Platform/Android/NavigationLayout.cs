﻿using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.CoordinatorLayout.Widget;

namespace Microsoft.Maui
{
	public class NavigationLayout : CoordinatorLayout
	{
#pragma warning disable CS0618 //FIXME: [Preserve] is obsolete
		[Preserve(Conditional = true)]
		public NavigationLayout(Context context) : base(context)
		{
		}

		[Preserve(Conditional = true)]
		public NavigationLayout(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		[Preserve(Conditional = true)]
		public NavigationLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		[Preserve(Conditional = true)]
		protected NavigationLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}
#pragma warning restore CS0618 //FIXME: [Preserve] is obsolete
	}
}
