using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WBZ.Classes;
using WBZ.Controls;
using WBZ.Helpers;

namespace WBZ.Modules.Attmisc
{
	/// <summary>
	/// Interaction logic for GroupsList.xaml
	/// </summary>
	public partial class GroupsList : Window
	{
		public GroupsList()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// New
		/// </summary>
		private void btnNew_Click(object sender, MouseButtonEventArgs e)
		{
			var window = new MsgWin(MsgWin.Type.InputBox, "Nowa grupa", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", "");
			if (window.ShowDialog() == true)
			{
				
			}
		}

		/// <summary>
		/// Duplicate
		/// </summary>
		private void btnDuplicate_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new AttributesClassesAdd(M.InstancesList[index], true);
				window.Show();

				var window = new MsgWin(MsgWin.Type.InputBox, $"Duplikowanie grupy: {M.InstancesList[index].Fullpath}", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", M.InstancesList[index].Fullpath);
				if (window.ShowDialog() == true)
				{

				}
			}*/
		}

		/// <summary>
		/// Edit
		/// </summary>
		private void btnEdit_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			foreach (int index in indexes)
			{
				var window = new AttributesClassesAdd(M.InstancesList[index], true);
				window.Show();

				var window = new MsgWin(MsgWin.Type.InputBox, $"Edycja grupy: {M.InstancesList[index].Fullpath}", "Podaj pełną ścieżkę grupy (oddzielaj znakiem /)", M.InstancesList[index].Fullpath);
				if (window.ShowDialog() == true)
				{

				}
			}*/
		}

		/// <summary>
		/// Delete
		/// </summary>
		private void btnDelete_Click(object sender, MouseButtonEventArgs e)
		{
			/*
			var indexes = dgList.SelectedItems.Cast<C_AttributeClass>().Select(x => M.InstancesList.IndexOf(x));
			if (indexes.Count<int>() > 0 && MessageBox.Show("Czy na pewno usunąć zaznaczone rekordy?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				foreach (int index in indexes)
					SQL.DeleteAttributeClass(M.InstancesList[index].ID);
				btnRefresh_Click(null, null);
			}
			*/
		}

		/// <summary>
		/// Refresh
		/// </summary>
		private async void btnRefresh_Click(object sender, MouseButtonEventArgs e)
		{
			await Task.Run(() => {
				//UpdateFilters();
				//M.TotalItems = SQL.CountInstances(M.INSTANCE_TYPE, M.FilterSQL);
				//M.InstancesList = SQL.ListAttributesClasses(M.FilterSQL, M.Limit, M.Page = 0 * M.Limit, "name", false);
			});
		}

		/// <summary>
		/// Close
		/// </summary>
		private void btnClose_Click(object sender, MouseButtonEventArgs e)
		{
			Close();
		}
	}
}
