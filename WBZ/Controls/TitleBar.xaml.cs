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
					btnResize.IsEnabled = false;
					btnResize.Visibility = Visibility.Collapsed;
				}

				if (win.ResizeMode == ResizeMode.NoResize)
				{
					btnMinimize.IsEnabled = false;
					btnMinimize.Visibility = Visibility.Collapsed;
				}
			}
			catch { }
		}

		public ImageSource SubIcon
		{
			get { return (ImageSource)GetValue(pSubIcon); }
			set { SetValue(pSubIcon, value); }
		}

		public static readonly DependencyProperty pSubIcon
			= DependencyProperty.Register(
				  nameof(SubIcon),
				  typeof(ImageSource),
				  typeof(TitleBar),
				  new PropertyMetadata(null)
			  );

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (win.WindowState == WindowState.Maximized)
				(btnResize.Content as Image).Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_restoredown.ico"));
			else
				(btnResize.Content as Image).Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/icon32_maximize.ico"));
		}

		private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ClickCount == 2 && btnResize.IsEnabled)
					miResize_Click(null, null);
				else
					win.DragMove();
			}
		}

		/// <summary>
		/// Default size
		/// </summary>
		private void miDefaultSize_Click(object sender, RoutedEventArgs e)
		{
			win.Height = win.MinHeight * 2.5;
			win.Width = win.MinWidth * 2.5;
		}

		/// <summary>
		/// Set center
		/// </summary>
		public void miSetCenter_Click(object sender, RoutedEventArgs e)
		{
			Rect workArea = SystemParameters.WorkArea;
			win.Left = (workArea.Width - win.Width) / 2 + workArea.Left;
			win.Top = (workArea.Height - win.Height) / 2 + workArea.Top;
		}

		/// <summary>
		/// Minimize
		/// </summary>
		private void miMinimize_Click(object sender, RoutedEventArgs e)
		{
			win.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Resize
		/// </summary>
		private void miResize_Click(object sender, RoutedEventArgs e)
		{
			if (win.WindowState == WindowState.Normal)
				win.WindowState = WindowState.Maximized;
			else
				win.WindowState = WindowState.Normal;

			miRestoreDown.IsEnabled = !miRestoreDown.IsEnabled;
			miMaximize.IsEnabled = !miMaximize.IsEnabled;
		}

		/// <summary>
		/// Close
		/// </summary>
		private void miClose_Click(object sender, RoutedEventArgs e)
		{
			win.Close();
		}
	}
}
