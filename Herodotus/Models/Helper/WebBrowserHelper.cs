/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 ноября 2025 06:53:19
 * Version: 1.0.3
 */

using System.Windows;
using System.Windows.Controls;

namespace Herodotus.Models.Helper
{
    public class WebBrowserHelper
    {
        public static readonly DependencyProperty BindableSourceProperty =
           DependencyProperty.RegisterAttached(
               "BindableSource",
               typeof(string),
               typeof(WebBrowserHelper),
               new PropertyMetadata(null, OnBindableSourceChanged));

        public static string GetBindableSource(WebBrowser webBrowser) =>
            (string)webBrowser.GetValue(BindableSourceProperty);

        public static void SetBindableSource(WebBrowser webBrowser, string value) =>
            webBrowser.SetValue(BindableSourceProperty, value);

        private static void OnBindableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser webBrowser && e.NewValue is string url)
            {
                if (!string.IsNullOrEmpty(url))
                    webBrowser.Navigate(url);
            }
        }
    }
}
