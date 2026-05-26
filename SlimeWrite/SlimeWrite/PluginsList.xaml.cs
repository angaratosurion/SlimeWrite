namespace SlimeWrite;

public partial class PluginsList : ContentPage
{
	public PluginsList()
	{
		InitializeComponent();
       this.PluginsCollectionView.ItemsSource = Core.SDK.PluginManager.Plugins;


    }
}