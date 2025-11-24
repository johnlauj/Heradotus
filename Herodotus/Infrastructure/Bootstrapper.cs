/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 ноября 2025 12:27:52
 * Version: 1.0.26
 */

using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using Herodotus.Views;
using Herodotus.ViewModels;
using Herodotus.Services.Scenario;
using Herodotus.Services.Scenario.Implements;
namespace Herodotus
{
    public class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IScenarioService, ScenarioService>();
            // окно закрытия приложения
            containerRegistry.RegisterDialog<StepCardView, StepCardViewModel>();
            containerRegistry.RegisterDialog<AddScenarioView, AddScenarioViewModel>();
        }
    }
}
