using FloatTool.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FloatTool.Views
{
	public sealed class AboutData
	{
		public string AuthorUrl { get; set; } = Utils.HOME_URL[..Utils.HOME_URL.LastIndexOf('/')];
		public string CurrentVersion { get; set; } = AppHelpers.VersionCode;
	}

	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			DataContext = new AboutData();

			InitializeComponent();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (e.GetPosition(this).Y < 40) DragMove();
		}

		private void WindowButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo { FileName = e.Uri.ToString(), UseShellExecute = true });
		}

		private void Hyperlink_CheckUpdates(object sender, RequestNavigateEventArgs e)
		{
			Task.Factory.StartNew(() =>
			{
				var update = Utils.CheckForUpdates().Result;
				if (update != null && update.TagName != AppHelpers.VersionCode)
				{
					Dispatcher.Invoke(new Action(() =>
					{
						Logger.Info("New version available");
						var updateWindow = new UpdateWindow(update)
						{
							Owner = this
						};
						updateWindow.ShowDialog();
					}));
				}
			});
		}
	}
}
