using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WBZ.Globals;
using MODULE_CLASS = WBZ.Models.M_Attachment;

namespace WBZ.Modules.Attachments
{
	/// <summary>
	/// Interaction logic for AttachmentsGallery.xaml
	/// </summary>
	public partial class AttachmentsGallery : Window
	{
		D_AttachmentsGallery D = new D_AttachmentsGallery();

		public AttachmentsGallery()
		{
			InitializeComponent();
			DataContext = D;
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Update filters
		/// </summary>
		private void UpdateFilters()
		{
			D.FilterSQL = $"LOWER(COALESCE(a.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"(LOWER(a.name) like '%.png' or LOWER(a.name) like '%.jpg') and ";

			D.FilterSQL = D.FilterSQL.TrimEnd(" and ".ToCharArray());
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
			D.Filters = new MODULE_CLASS();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				D.TotalItems = SQL.CountInstances(D.MODULE_TYPE, D.FilterSQL);
				D.InstancesList = SQL.ListInstances<MODULE_CLASS>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, D.Page = 0);

				foreach (var img in D.InstancesList)
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

		/// <summary>
		/// Images - SelectionChanged
		/// </summary>
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
			if (e.HorizontalChange > 0 && e.HorizontalOffset + e.ViewportWidth == e.ExtentWidth && D.InstancesList.Count < D.TotalItems)
			{
				DataContext = null;
				D.InstancesList.AddRange(SQL.ListInstances<MODULE_CLASS>(D.MODULE_TYPE, D.FilterSQL, D.SORTING, ++D.Page));
				foreach (var img in D.InstancesList)
					img.File = SQL.GetAttachmentFile(img.ID);
				DataContext = D;
				Extensions.GetVisualChild<ScrollViewer>(sender as DataGrid).ScrollToHorizontalOffset(e.HorizontalOffset);
			}
		}

		/// <summary>
		/// Closed
		/// </summary>
		private void Window_Closed(object sender, EventArgs e)
		{
			try
			{
				if (D.InstancesList != null)
					foreach (var attachment in D.InstancesList)
						if (attachment.File != null)
							File.Delete(Path.Combine(Path.GetTempPath(), attachment.Name));
			}
			catch { }
		}
	}
}
