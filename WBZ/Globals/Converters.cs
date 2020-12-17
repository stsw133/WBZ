using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WBZ.Globals
{
	/// <summary>
	/// Convert bool -> !bool , bool -> !Visibility : parameter must be bool
	/// </summary>
	public class conv_BoolInverted : MarkupExtension, IValueConverter
	{
		private static conv_BoolInverted _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_BoolInverted();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType.Name == "Visibility")
				return !System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
			else
				return !System.Convert.ToBoolean(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType.Name == "Visibility")
				return !System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
			else
				return !System.Convert.ToBoolean(value);
		}
	}

	/// <summary>
	/// Convert bool -> string : parameter must be like 'string~string'
	/// If true then string is on left side of ~ else on right side
	/// </summary>
	public class conv_BoolToString : MarkupExtension, IValueConverter
	{
		private static conv_BoolToString _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_BoolToString();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value) return parameter.ToString().Split('~')[0];
			return parameter.ToString().Split('~')[1];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return parameter.ToString().Split('~')[1];
		}
	}

	/// <summary>
	/// Convert bool -> Visibility : parameter must be bool
	/// </summary>
	public class conv_BoolToVisibility : MarkupExtension, IValueConverter
	{
		private static conv_BoolToVisibility _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_BoolToVisibility();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
		}
	}

	/// <summary>
	/// Compare string to string : parameter must be string
	/// </summary>
	public class conv_CompareStrings : MarkupExtension, IValueConverter
	{
		private static conv_CompareStrings _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_CompareStrings();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value?.ToString() == parameter?.ToString())
			{
				if (targetType.Name == "Visibility")
					return Visibility.Visible;
				else
					return true;
			}
			else
			{
				if (targetType.Name == "Visibility")
					return Visibility.Collapsed;
				else
					return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType.Name == "Visibility")
				return Visibility.Collapsed;
			else
				return false;
			//return value;
		}
	}

	/// <summary>
	/// Convert list.contains(string) : parameter must be string
	/// If true then returns true or Visibility.Visible else returns false or Visibility.Collapsed
	/// </summary>
	public class conv_ListContains : MarkupExtension, IValueConverter
	{
		private static conv_ListContains _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_ListContains();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((value as List<string>).Contains(parameter.ToString().TrimStart('!')))
			{
				if (targetType.Name == "Visibility")
					return parameter.ToString()[0] != '!' ? Visibility.Visible : Visibility.Collapsed;
				else
					return parameter.ToString()[0] != '!' ? true : false;
			}
			else
			{
				if (targetType.Name == "Visibility")
					return parameter.ToString()[0] != '!' ? Visibility.Collapsed : Visibility.Visible;
				else
					return parameter.ToString()[0] != '!' ? false : true;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType.Name == "Visibility")
				return Visibility.Collapsed;
			else
				return false;
		}
	}

	/// <summary>
	/// Convert double -> double * double : parameter must be number
	/// </summary>
	public class conv_Size : MarkupExtension, IValueConverter
	{
		private static conv_Size _conv = null;
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_conv == null)
				_conv = new conv_Size();
			return _conv;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value, CultureInfo.InvariantCulture) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value, CultureInfo.InvariantCulture) / System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
		}
	}
}
