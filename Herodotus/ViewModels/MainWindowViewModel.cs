/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 ноября 2025 06:53:20
 * Version: 1.0.17
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using DryIoc;

using Herodotus.Models;
using Herodotus.Services.Scenario;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
namespace Herodotus.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Herodotus";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private int _stepIndex;
        public int StepIndex
        {
            get => _stepIndex;
            set => SetProperty(ref _stepIndex, value);
        }

        private ScenarioModel _selectedScenario;
        public ScenarioModel SelectedScenario
        {
            get => _selectedScenario;
            set => SetProperty(ref _selectedScenario, value);
        }

        private StepItemModel _activeStepCard;
        private readonly IDialogService _dialogService;
        private readonly IScenarioService _scenarioService;

        public StepItemModel ActiveStepCard
        {
            get => _activeStepCard;
            set => SetProperty(ref _activeStepCard, value);
        }

        public ObservableCollection<ScenarioModel> Scenarios { get; }
                = new ObservableCollection<ScenarioModel>();

        public ObservableCollection<ScenarioModel> AllScenarios { get; }
                = new ObservableCollection<ScenarioModel>();

        public DelegateCommand AddScenarioCommand { get; }
        public DelegateCommand ChangeOrDeleteScenarioCommand { get; }
        public DelegateCommand OpenScenarioCommand { get; }
        public DelegateCommand<ScenarioModel> DeleteScenarioCommand { get; }
        public DelegateCommand<StepItemModel> ShowStepCardCommand { get; }

        public MainWindowViewModel(IDialogService dialogService,
             IScenarioService scenarioService)
        {
            _dialogService = dialogService;
            _scenarioService = scenarioService;

            // загружаю список доступных сценариев в Combobox выбора
            var scenarios = _scenarioService.LoadScenarios();
            AllScenarios.AddRange(scenarios);

            // инициалиазция добавления сценария
            AddScenarioCommand = new DelegateCommand(AddScenario);
            // инициалиазция изменения/удаления сценария
            ChangeOrDeleteScenarioCommand = new DelegateCommand(EditScenario);
            // инициализация открытия сценария из combobox по выбору
            OpenScenarioCommand = new DelegateCommand(OpenScenario());
            // инициализация удаления открытого сценария из рабочего пространства
            DeleteScenarioCommand = new DelegateCommand<ScenarioModel>(DeleteScenarioView());
            // инициализация открытия данных этапа сценария
            ShowStepCardCommand = new DelegateCommand<StepItemModel>(OpenCardStepScenario());
        }

        private Action<StepItemModel> OpenCardStepScenario()
        {
            return step =>
            {
                var parameters = new DialogParameters();
                parameters.Add("step", step); // "step" — ключ, StepItem — значение

                _dialogService.ShowDialog("StepCardView", parameters, r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        // Получаем изменённый шаг из диалога
                        if (r.Parameters.TryGetValue("step", out StepItemModel editedStep))
                        {
                            // Обновляем оригинальный объект
                            step.Header = editedStep.Header;
                            step.Content = editedStep.Content;
                            step.Description = editedStep.Description;

                            // Сохраняем актуальный список
                            _scenarioService.SaveScenarios(AllScenarios);
                        }
                    }
                });
            };
        }

        private Action<ScenarioModel> DeleteScenarioView()
        {
            return scenario =>
            {
                if (scenario == null) return;
                Scenarios.Remove(scenario);
            };
        }

        private Action OpenScenario()
        {
            return () =>
            {
                if (SelectedScenario == null) return;

                // Создаём сценарий на основе выбранного шаблона
                var scenario = new ScenarioModel
                {
                    Name = SelectedScenario.Name,
                    Steps = new ObservableCollection<StepItemModel>(SelectedScenario.Steps)
                };

                // Добавляем в список сценариев
                Scenarios.Add(scenario);

                // Если нужно — сразу выбрать его
                SelectedScenario = scenario;
            };
        }

        private void AddScenario()
        {
            _dialogService.ShowDialog("AddScenarioView", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var scenarioName = r.Parameters.GetValue<string>("ScenarioName");
                    var scenarioSteps = r.Parameters.GetValue<ObservableCollection<StepItemModel>>("Steps");

                    var scenario = new ScenarioModel()
                    {
                        Name = scenarioName,
                        Steps = scenarioSteps
                    };

                    // Сохраняем на диск
                    _scenarioService.SaveScenario(scenario);

                    // Добавляем в коллекцию на экране
                    Scenarios.Add(scenario);
                    AllScenarios.Add(scenario);
                    SelectedScenario = scenario;
                }
            });
        }

        private void EditScenario()
        {
            if (SelectedScenario == null) return;

            var parameters = new DialogParameters
            {
                { "Scenario", SelectedScenario }
            };

            _dialogService.ShowDialog("AddScenarioView", parameters, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var scenarioName = r.Parameters.GetValue<string>("ScenarioName");
                    var scenarioDelete = r.Parameters.GetValue<bool>("Delete");
                    var scenarioSteps = r.Parameters.GetValue<ObservableCollection<StepItemModel>>("Steps");
                    if(scenarioDelete)
                    {
                        var scenario = AllScenarios.FirstOrDefault(it => it.Name == scenarioName);
                        if (scenario != null)
                        {
                            AllScenarios.Remove(scenario);

                            // Если у тебя есть отдельная коллекция для отображения на экране
                            Scenarios.Remove(scenario);

                            // Сохраняем актуальный список
                            _scenarioService.SaveScenarios(AllScenarios);

                            // удаляет файл сценария
                            _scenarioService.DeleteScenario(scenario.Name);
                        }
                    }

                    var updatedScenario = new ScenarioModel()
                    {
                        Name = scenarioName,
                        Steps = scenarioSteps
                    };

                    // Сохраняем на диск
                    _scenarioService.SaveScenario(updatedScenario);

                    // Обновляем коллекцию
                    var index = Scenarios.IndexOf(SelectedScenario);
                    if (index >= 0)
                        Scenarios[index] = updatedScenario;

                    var allIndex = AllScenarios.IndexOf(SelectedScenario);
                    if (allIndex >= 0)
                        AllScenarios[allIndex] = updatedScenario;

                    SelectedScenario = updatedScenario;
                }
            });
        }
    }
}
