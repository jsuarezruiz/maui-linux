﻿using System;
using Android.Content;

namespace Microsoft.Maui
{
	public partial class MauiContext
	{
		readonly WeakReference<Context>? _context;

		public MauiContext(IServiceProvider services, Context context)
			: this(services)
		{
			_context = new WeakReference<Context>(context ?? throw new ArgumentNullException(nameof(context)));
		}

		public MauiContext(Context context)
			: this()
		{
			_context = new WeakReference<Context>(context ?? throw new ArgumentNullException(nameof(context)));
		}

		public Context? Context
		{
			get
			{
				if (_context == null)
					return null;

				return _context.TryGetTarget(out Context? context) ? context : null;
			}
		}
	}
}
