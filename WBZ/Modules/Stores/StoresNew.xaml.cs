using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WBZ.Models;
using WBZ.Helpers;
using WBZ.Modules.Articles;
using WBZ.Modules.Documents;
using MODULE_CLASS = WBZ.Models.C_Store;

namespace WBZ.Modules.Stores
{
	/// <summary>
	/// Interaction logic for StoresNew.xaml
	/// </summary>
	public partial class StoresNew : Window
	{
		D_StoresNew D = new D_StoresNew();

		public StoresNew(MODULE_CLASS instance, Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;

			D.InstanceInfo = instance;
			D.Mode = mode;

			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE))
				D.InstanceInfo.ID = SQL.NewInstanceID(D.MODULE_NAME);
		}

		/// <summary>
		/// Validation
		/// </summary>
		private bool CheckDataValidation()
		{
			bool result = true;
			
			return result;
		}

		/// <summary>
		/// Save
		/// </summary>
		private bool saved = false;
		private void btnSave_Click(object sender, MouseButtonEventArgs e)
		{
			if (!CheckDataValidation())
				return;

			if (saved = SQL.SetInstance(D.MODULE_NAME, D.InstanceInfo, D.Mode))
				Close();
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			if (D.InstanceInfo.ID == 0)
				return;
			//TODO - dorobić odświeżanie zmienionych danych
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Tab changed
		/// </summary>
		private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tab = (e.AddedItems.Count > 0 ? e.AddedItems[0] : null) as TabItem;
			if (tab?.Name == "tabSources_Articles")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Articles == null)
					D.InstanceSources_Articles = SQL.ListInstances<C_Article>(Global.Module.ARTICLES, $"sa.store={D.InstanceInfo.ID}");
			}
			else if (tab?.Name == "tabSources_Documents")
			{
				if (D.InstanceInfo.ID != 0 && D.InstanceSources_Documents == null)
					D.InstanceSources_Documents = SQL.ListInstances<C_Document>(Global.Module.DOCUMENTS, $"d.store={D.InstanceInfo.ID}");
			}
		}

		/// <summary>
		/// Open: Article
		/// </summary>
		private void dgList_Articles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.ARTICLES}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Article>();
				foreach (C_Article instance in selectedInstances)
				{
					var window = new ArticlesNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Open: Document
		/// </summary>
		private void dgList_Documents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Commands.Type perm = Global.User.Perms.Contains($"{Global.Module.DOCUMENTS}_{Global.UserPermType.SAVE}")
					? Commands.Type.EDIT : Commands.Type.PREVIEW;

				var selectedInstances = (sender as DataGrid).SelectedItems.Cast<C_Document>();
				foreach (C_Document instance in selectedInstances)
				{
					var window = new DocumentsNew(instance, perm);
					window.Show();
				}
			}
		}

		/// <summary>
		/// Closed
		/// </summary>
		private void Window_Closed(object sender, EventArgs e)
		{
			if (D.Mode.In(Commands.Type.NEW, Commands.Type.DUPLICATE) && !saved)
				SQL.ClearObject(D.MODULE_NAME, D.InstanceInfo.ID);
		}
	}
}
