/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 ноября 2025 12:27:52
 * Version: 1.0.26
 */

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using HandyControl.Controls;
using HandyControl.Themes;
namespace Herodotus
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;

            base.OnStartup(e);
            var boot = new Bootstrapper();
            boot.Run();
        }

        internal void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        internal void UpdateAccent(Brush accent)
        {
            if (ThemeManager.Current.AccentColor != accent)
            {
                ThemeManager.Current.AccentColor = accent;
            }
        }
    }
}
