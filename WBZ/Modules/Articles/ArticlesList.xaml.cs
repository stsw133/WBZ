using System.Windows.Controls;
using WBZ.Globals;
using WBZ.Interfaces;
using WBZ.Models;
using MODULE_MODEL = WBZ.Models.M_Article;

namespace WBZ.Modules.Articles
{
	/// <summary>
	/// Interaction logic for ArticlesList.xaml
	/// </summary>
	public partial class ArticlesList : List
	{
		D_ArticlesList D = new D_ArticlesList();

		public ArticlesList(Commands.Type mode)
		{
			InitializeComponent();
			DataContext = D;
			Init();

			D.Mode = mode;
			if (D.StoresList.Rows.Count > 0)
				cbStore.SelectedIndex = 0;
		}

		/// Selected
		public MODULE_MODEL Selected;

		/// <summary>
		/// Store - SelectionChanged
		/// </summary>
		public M_Store SelectedStore;
		private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedStore = SQL.GetInstance<M_Store>(Global.Module.STORES, cbStore.SelectedValue != null ? (int)cbStore.SelectedValue : 0);
			btnRefresh_Click(null, null);
		}
	}

	public class List : ModuleList<MODULE_MODEL> { }
}
