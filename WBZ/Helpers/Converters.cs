using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WBZ.Helpers
{
	/// <summary>
	/// Konwerter wartości bool na string, parametr musi składać się z dwóch ciągów oddzielonych znakiem ~
	/// Jeśli wartość=true to podstawiany jest ciąg z lewej strony parametru, dla wartości false z prawej strony
	/// </summary>
	public class conv_BoolToString : IValueConverter
	{
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
	/// Konwerter wartości bool na string, parametr musi składać się z dwóch ciągów oddzielonych znakiem ~
	/// Jeśli wartość=true to podstawiany jest ciąg z lewej strony parametru, dla wartości false z prawej strony
	/// </summary>
	public class conv_BoolInverted : IValueConverter
	{
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
	/// Jeśli wartość parametru jest zawarta w liście wartości źródła to konwerter zwraca: true lub Visibility.Visible
	/// W przeciwnym wypadku zwraca: false lub Visibility.Collapsed
	/// </summary>
	public class conv_ListContains : IValueConverter
	{
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
					return parameter.ToString()[0] != '!' ? Visibility.Collapsed : Visibility.Collapsed;
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
	/// Konwerter wartości int, parametr jest mnożnikiem np. jeśli wartość=3 a parametr=5 to w wyniku wartość wyniesie 15
	/// </summary>
	public class conv_Size : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value, CultureInfo.InvariantCulture) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return System.Convert.ToDouble(value, CultureInfo.InvariantCulture) / System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
		}
	}

	/// <summary>
	/// Konwerter wartości string na Visibility
	/// </summary>
	public class conv_StringToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.ToString() == "True")
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.ToString() != "True")
				return Visibility.Visible;
			else
				return Visibility.Collapsed;
		}
	}
}
