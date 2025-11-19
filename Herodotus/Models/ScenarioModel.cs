/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 ноября 2025 08:17:19
 * Version: 1.0.21
 */

using System.Collections.ObjectModel;

using Prism.Commands;
using Prism.Mvvm;

namespace Herodotus.Models
{
    public class ScenarioModel : BindableBase
    {
        public string Name { get; set; }
        public ObservableCollection<StepItemModel> Steps { get; set; } = new ObservableCollection<StepItemModel>();

        private int _stepIndex;
        public int StepIndex
        {
            get => _stepIndex;
            set => SetProperty(ref _stepIndex, value);
        }

        public DelegateCommand PrevCmd { get; }
        public DelegateCommand NextCmd { get; }

        public ScenarioModel()
        {
            PrevCmd = new DelegateCommand(() =>
            {
                if (StepIndex > 0) StepIndex--;
            });
            NextCmd = new DelegateCommand(() =>
            {
                if (StepIndex < Steps.Count - 1) StepIndex++;
            });
        }
    }

    /// <summary>
    /// DTO класс для сериализации
    /// </summary>
    public class ScenarioDto
    {
        public string Name { get; set; }
        public ObservableCollection<StepItemModel> Steps { get; set; } = new ObservableCollection<StepItemModel>();
    }
}
