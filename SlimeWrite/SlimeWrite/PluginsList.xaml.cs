using SlimeWrite.Core.SDK.Interfaces;

namespace SlimeWrite;

public partial class PluginsList : ContentPage
{
	public PluginsList()
	{
		InitializeComponent();
       this.PluginsCollectionView.ItemsSource = Core.SDK.PluginManager.Plugins;
    }
    private void OnSettingsClicked(object sender, EventArgs e)
    {
        // 1. Καταλαβαίνουμε ποιο κουμπί πατήθηκε
        var button = sender as Button;

        // 2. Παίρνουμε το BindingContext του κουμπιού, που είναι το συγκεκριμένο ISlimePlugin αυτού του row
        var plugin = button?.BindingContext as ISlimePlugin;

        if (plugin != null)
        {
            // 3. Καλείς τη μέθοδο του interface περνώντας την τρέχουσα σελίδα (this)
            plugin.OpenPluginSettings(this);
        }
    }
}