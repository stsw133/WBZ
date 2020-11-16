using System.Windows.Controls;
using WBZ.Helpers;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.C_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesList.xaml
	/// </summary>
	public partial class ArticlesList : _ArticlesList
	{
		D_ArticlesList D = new D_ArticlesList();

		public ArticlesList(bool selectingMode = false)
		{
			InitializeComponent();
			DataContext = D;

			D.SelectingMode = selectingMode;
		}

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		public C_Store SelectedStore;
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance<C_Store>(Global.Module.STORES, cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0);
			btnRefresh_Click(null, null);
		}

		/// <summary>
		/// Select
		/// </summary>
		public MODULE_MODEL Selected;
	}

	public class _ArticlesList : ModuleList<MODULE_MODEL> { }
}
