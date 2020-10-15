using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WBZ.Classes;
using WBZ.Helpers;
using MODULE_CLASS = WBZ.Classes.C_Attachment;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for AttachmentsGallery.xaml
	/// </summary>
	public partial class AttachmentsGallery : Window
	{
		M_AttachmentsGallery M = new M_AttachmentsGallery();

		public AttachmentsGallery()
		{
			InitializeComponent();
			DataContext = M;
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			M.FilterSQL = $"LOWER(COALESCE(a.module,'')) like '%{M.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{M.Filters.Name.ToLower()}%' and "
						+ $"(LOWER(a.name) like '%.png' or LOWER(a.name) like '%.jpg') and ";

			M.FilterSQL = M.FilterSQL.TrimEnd(" and ".ToCharArray());
		}

		/// <summary>
		/// Apply filters
		/// </summary>
		private void dpFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Clear filters
		/// </summary>
		private void btnFiltersClear_Click(object sender, MouseButtonEventArgs e)
		{
			M.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				M.TotalItems = SQL.CountInstances(M.MODULE_NAME, M.FilterSQL);
				M.InstancesList = SQL.ListInstances(M.MODULE_NAME, M.FilterSQL, M.SORTING, M.Page = 0).DataTableToList<MODULE_CLASS>();

				foreach (var img in M.InstancesList)
					img.File = SQL.GetAttachmentFile(img.ID);
			});
		}

		private void btnStretchNone(object sender, RoutedEventArgs e)
		{
			imgContent.Stretch = Stretch.None;
		}

		private void btnStretchUniform(object sender, RoutedEventArgs e)
		{
			imgContent.Stretch = Stretch.Uniform;
		}

		private void lbImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var lbImages = (sender as ListBox);

			if (lbImages.SelectedIndex < 0)
				return;

			var selection = lbImages.SelectedItem as MODULE_CLASS;
			if (selection.File == null)
				selection.File = SQL.GetAttachmentFile(selection.ID);

			using (var stream = new MemoryStream(selection.File))
			{
				imgContent.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
			}
		}

		/// <summary>
		/// Load more
		/// </summary>
		private void lbImages_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.HorizontalChange > 0 && e.HorizontalOffset + e.ViewportWidth == e.ExtentWidth && M.InstancesList.Count < M.TotalItems)
			{
				DataContext = null;
				M.InstancesList.AddRange(SQL.ListInstances(M.MODULE_NAME, M.FilterSQL, M.SORTING, ++M.Page).DataTableToList<MODULE_CLASS>());
				foreach (var img in M.InstancesList)
					img.File = SQL.GetAttachmentFile(img.ID);
				DataContext = M;
				Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToHorizontalOffset(e.HorizontalOffset);
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			try
			{
				if (M.InstancesList != null)
					foreach (var attachment in M.InstancesList)
						if (attachment.File != null)
							File.Delete(Path.Combine(Path.GetTempPath(), attachment.Name));
			}
			catch { }
		}
	}

	/// <summary>
	/// Model
	/// </summary>
	internal class M_AttachmentsGallery : INotifyPropertyChanged
	{
		public readonly string MODULE_NAME = Global.Module.ATTACHMENTS;
		public StringCollection SORTING = Properties.Settings.Default.sorting_AttachmentsGallery;

		/// Logged user
		public C_User User { get; } = Global.User;
		/// Instances list
		private List<MODULE_CLASS> instancesList;
		public List<MODULE_CLASS> InstancesList
		{
			get
			{
				return instancesList;
			}
			set
			{
				instancesList = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Selecting mode
		public bool SelectingMode { get; set; }
		/// SQL filter
		public string FilterSQL { get; set; }
		/// Filter instance
		private MODULE_CLASS filters = new MODULE_CLASS();
		public MODULE_CLASS Filters
		{
			get
			{
				return filters;
			}
			set
			{
				filters = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Page number
		private int page;
		public int Page
		{
			get
			{
				return page;
			}
			set
			{
				page = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}
		/// Total instances number
		private int totalItems;
		public int TotalItems
		{
			get
			{
				return totalItems;
			}
			set
			{
				totalItems = value;
				NotifyPropertyChanged(MethodBase.GetCurrentMethod().Name.Substring(4));
			}
		}

		/// <summary>
		/// PropertyChangedEventHandler
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
