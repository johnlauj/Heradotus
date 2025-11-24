/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 ноября 2025 12:27:52
 * Version: 1.0.26
 */

using System;
using System.Windows.Input;

using Herodotus.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Herodotus.ViewModels
{
    public class StepCardViewModel : BindableBase, IDialogAware
    {
        public string Header { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }

        private StepItemModel _step;
        public StepItemModel Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }

        public ICommand CloseCommand { get; }
        public DelegateCommand SaveCommand { get; }

        public string Title => Content;

        public event Action<IDialogResult> RequestClose;

        public StepCardViewModel()
        {
            // Закрываем диалог через RequestClose
            CloseCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });

            SaveCommand = new DelegateCommand(() =>
            {
                var p = new DialogParameters { { "step", Step } };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, p));
            });
        }

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue("step", out StepItemModel step))
            {
                Step = step;
                Header = step.Header;
                Content = step.Content;
                Description = step.Description;
            }
        }
    }
}
