using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MODULE_MODEL = WBZ.Models.M_Attachment;

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
			D.FilterSqlString = $"LOWER(COALESCE(a.module,'')) like '%{D.Filters.Module.ToLower()}%' and "
						+ $"LOWER(COALESCE(a.name,'')) like '%{D.Filters.Name.ToLower()}%' and "
						+ $"(LOWER(a.name) like '%.png' or LOWER(a.name) like '%.jpg') and ";

			D.FilterSqlString = D.FilterSqlString.TrimEnd(" and ".ToCharArray());
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
		private void btnFiltersClear_Click(object sender, RoutedEventArgs e)
		{
			D.Filters = new MODULE_MODEL();
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			await Task.Run(() => {
				UpdateFilters();
				D.TotalItems = SQL.CountInstances(D.Module, D.FilterSqlString);
				D.InstancesList = SQL.ListInstances<MODULE_MODEL>(D.Module, D.FilterSqlString, D.FilterSqlParams, D.Sorting, D.InstancesList?.Count ?? 0);

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

			var selection = lbImages.SelectedItem as MODULE_MODEL;
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
				foreach (var i in SQL.ListInstances<MODULE_MODEL>(D.Module, D.FilterSqlString, D.FilterSqlParams, D.Sorting, D.InstancesList?.Count ?? 0)) D.InstancesList.Add(i);
				foreach (var img in D.InstancesList)
					img.File = SQL.GetAttachmentFile(img.ID);
				(e.OriginalSource as ScrollViewer).ScrollToVerticalOffset(e.HorizontalOffset);
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
