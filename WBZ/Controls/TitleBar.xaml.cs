﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WBZ.Controls
{
	/// <summary>
	/// Interaction logic for TitleBar.xaml
	/// </summary>
	public partial class TitleBar : UserControl
	{
		public TitleBar()
		{
			InitializeComponent();
		}

		Window win;

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				win = Window.GetWindow(this);
				win.StateChanged += new EventHandler(Window_StateChanged);

				if ((win.ResizeMode == ResizeMode.NoResize)
				|| (win.ResizeMode == ResizeMode.CanMinimize))
				{
					titleBar_Resize.IsEnabled = false;
					titleBar_Resize.Visibility = Visibility.Collapsed;
				}

				if (win.ResizeMode == ResizeMode.NoResize)
				{
					titleBar_Minimize.IsEnabled = false;
					titleBar_Minimize.Visibility = Visibility.Collapsed;
				}
			}
			catch { }
		}

		public ImageSource AdditionalIcon
		{
			get { return (ImageSource)GetValue(pAdditionalIcon); }
			set { SetValue(pAdditionalIcon, value); }
		}

		public static readonly DependencyProperty pAdditionalIcon
			= DependencyProperty.Register(
				  nameof(AdditionalIcon),
				  typeof(ImageSource),
				  typeof(TitleBar),
				  new PropertyMetadata(new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/_null.ico")))
			  );

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (win.WindowState == WindowState.Maximized)
				((Image)titleBar_Resize.Content).Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_restoredown.ico"));
			else
				((Image)titleBar_Resize.Content).Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_maximize.ico"));
		}

		private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ClickCount == 2 && titleBar_Resize.IsEnabled)
					titleBar_Resize_Click(null, null);
				else
					win.DragMove();
			}
		}

		/// <summary>
		/// Default size
		/// </summary>
		private void titleBar_DefaultSize_Click(object sender, RoutedEventArgs e)
		{
			win.Height = win.MinHeight * 2.5;
			win.Width = win.MinWidth * 2.5;
		}

		/// <summary>
		/// Set center
		/// </summary>
		public void titleBar_SetCenter_Click(object sender, RoutedEventArgs e)
		{
			Rect workArea = SystemParameters.WorkArea;
			win.Left = (workArea.Width - win.Width) / 2 + workArea.Left;
			win.Top = (workArea.Height - win.Height) / 2 + workArea.Top;
		}

		/// <summary>
		/// Minimize
		/// </summary>
		private void titleBar_Minimize_Click(object sender, RoutedEventArgs e)
		{
			win.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Resize
		/// </summary>
		private void titleBar_Resize_Click(object sender, RoutedEventArgs e)
		{
			if (win.WindowState == WindowState.Normal)
				win.WindowState = WindowState.Maximized;
			else
				win.WindowState = WindowState.Normal;

			titleBar_Menu_RestoreDown.IsEnabled = !titleBar_Menu_RestoreDown.IsEnabled;
			titleBar_Menu_Maximize.IsEnabled = !titleBar_Menu_Maximize.IsEnabled;
		}

		/// <summary>
		/// Close
		/// </summary>
		private void titleBar_Close_Click(object sender, RoutedEventArgs e)
		{
			win.Close();
		}
	}
}
