/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 ноября 2025 06:53:19
 * Version: 1.0.3
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using HandyControl.Controls;

using Herodotus.Models;

namespace Herodotus.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StackPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is StackPanel sp && sp.DataContext is StepItemModel step)
            {
                var sb = FindParent < StepBar > (sp);
                if (sb != null && sb.DataContext is ScenarioModel scenario)
                {
                    scenario.StepIndex = scenario.Steps.IndexOf(step);

                    // Прокрутка ScrollViewer к выбранному элементу
                    sp.BringIntoView();
                }
            }
        }


        // Вспомогательная функция
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            if (parent is T t) return t;
            return FindParent<T>(parent);
        }
    }
}
