﻿namespace Microsoft.Maui.Controls
{
	public partial class DatePicker : IDatePicker
	{
		Font? _font;

		Font ITextStyle.Font => _font ??= this.ToFont();
	}
}