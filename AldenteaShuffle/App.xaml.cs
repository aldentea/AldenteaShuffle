using System.Configuration;
using System.Data;
using System.Windows;

namespace Aldentea.AldenteaShuffle
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		internal static Settings MySetting => Settings.Default;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (MySetting.RequireUpgrade)
			{
				MySetting.Upgrade();
				MySetting.RequireUpgrade = false;
			}
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			MySetting.Save();
		}
	}

}
