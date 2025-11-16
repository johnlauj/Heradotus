/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 ноября 2025 06:53:16
 * Version: 1.0.18
 */

using System;
using System.Collections.ObjectModel;

using Herodotus.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Herodotus.ViewModels
{
    internal class AddScenarioViewModel : BindableBase, IDialogAware
    {
        private string _scenarioName;
        public string ScenarioName
        {
            get => _scenarioName;
            set => SetProperty(ref _scenarioName, value);
        }

        public ObservableCollection<StepItemModel> Steps { get; } = new ObservableCollection<StepItemModel>();

        public DelegateCommand AddStepCommand { get; }
        public DelegateCommand<StepItemModel> RemoveStepCommand { get; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public string Title => "Создание сценария";

        public event Action<IDialogResult> RequestClose;

        public AddScenarioViewModel()
        {
            AddStepCommand = new DelegateCommand(() =>
            {
                Steps.Add(new StepItemModel { Header = "Этап " + Steps.Count, Content = "", Description = "" });
            });

            RemoveStepCommand = new DelegateCommand<StepItemModel>(step =>
            {
                if (step != null)
                    Steps.Remove(step);
            });

            SaveCommand = new DelegateCommand(SaveScenario());
            DeleteCommand = new DelegateCommand(DeleteScenario);

            CancelCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        private Action SaveScenario()
        {
            return () =>
            {
                if (!string.IsNullOrEmpty(ScenarioName)
                    && Steps.Count > 0)
                {
                    var p = new DialogParameters
                    {
                        { "ScenarioName", ScenarioName },
                        { "Steps", Steps },
                        { "Delete", false }
                    };
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, p));
                }
            };
        }

        private void DeleteScenario()
        {
            if (!string.IsNullOrEmpty(ScenarioName)
                    && Steps.Count > 0)
            {
                var p = new DialogParameters
                {
                    { "ScenarioName", ScenarioName },
                    { "Delete", true }
                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, p));
            }
        }

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Scenario"))
            {
                var scenario = parameters.GetValue<ScenarioModel>("Scenario");
                ScenarioName = scenario.Name;

                Steps.Clear();
                foreach (var s in scenario.Steps)
                    Steps.Add(new StepItemModel
                    {
                        Header = s.Header,
                        Content = s.Content,
                        Description = s.Description
                    });
            }
        }
    }
}
