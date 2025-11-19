/*
 * Author: Nikolay Dvurechensky
 * Site: https://sites.google.com/view/dvurechensky
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 ноября 2025 08:17:19
 * Version: 1.0.21
 */

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Herodotus.Models;

namespace Herodotus.Services.Scenario.Implements
{
    public class ScenarioService : IScenarioService
    {
        private string _scenariosFolder;

        public ScenarioService() {
            _scenariosFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
            if (!Directory.Exists(_scenariosFolder))
                Directory.CreateDirectory(_scenariosFolder);
        }

        /// <summary>
        /// Загружает все сценарии из папки Scenarios
        /// </summary>
        public ObservableCollection<ScenarioModel> LoadScenarios()
        {
            var scenarios = new ObservableCollection<ScenarioModel>();

            var files = Directory.GetFiles(_scenariosFolder, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var scenarioDto = JsonSerializer.Deserialize<ScenarioDto>(json);

                    if(!string.IsNullOrEmpty(scenarioDto.Name)
                        && (scenarioDto.Steps == null || scenarioDto.Steps.Count == 0))
                    {
                        DeleteScenario(scenarioDto.Name);
                        continue;
                    }

                    if (scenarioDto != null && scenarioDto.Steps != null)
                    {
                        var scenario = new ScenarioModel
                        {
                            Name = scenarioDto.Name,
                            Steps = new ObservableCollection<StepItemModel>(scenarioDto.Steps)
                        };
                        scenarios.Add(scenario);
                    }
                }
                catch (Exception ex)
                {
                    // можно логировать ошибки
                }
            }

            return scenarios;
        }

        /// <summary>
        /// Сохраняет сценарий в папку Scenarios
        /// </summary>
        public void SaveScenario(ScenarioModel scenario)
        {
            if (scenario == null) return;

            var fileName = $"{scenario.Name}.json";
            var filePath = Path.Combine(_scenariosFolder, fileName);

            var dto = new ScenarioDto
            {
                Name = scenario.Name,
                Steps = scenario.Steps
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(dto, options);

            File.WriteAllText(filePath, json);
            SaveScenarioAsMarkdown(scenario, _scenariosFolder);
        }

        public static void SaveScenarioAsMarkdown(ScenarioModel scenario, string folder)
        {
            if (scenario == null) return;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{scenario.Name}.md";
            var filePath = Path.Combine(folder, fileName);

            var sb = new StringBuilder();

            // Заголовок сценария
            sb.AppendLine($"# Сценарий: {scenario.Name}");
            sb.AppendLine();
            sb.AppendLine("## Общая информация");
            sb.AppendLine($"- **Имя сценария:** {scenario.Name}");
            sb.AppendLine($"- **Текущий шаг:** {scenario.StepIndex}");
            sb.AppendLine();
            sb.AppendLine("## Шаги сценария");

            // Все шаги
            for (int i = 0; i < scenario.Steps.Count; i++)
            {
                var step = scenario.Steps[i];
                sb.AppendLine();
                sb.AppendLine($"### Шаг {i + 1}");
                sb.AppendLine($"- **Заголовок:** {step.Header}");
                sb.AppendLine($"- **Содержание:** {step.Content}");
                sb.AppendLine($"- **Описание:** {step.Description}");
            }

            // Сохраняем в файл
            File.WriteAllText(filePath, sb.ToString());
            Console.WriteLine($"Markdown сценария сохранён: {filePath}");
        }

        /// <summary>
        /// Удаляет сценарий: из коллекции и из файла
        /// </summary>
        public bool DeleteScenario(string scenarioName, ObservableCollection<ScenarioModel> scenarios = null)
        {
            if (string.IsNullOrWhiteSpace(scenarioName))
                return false;

            // Удаляем из коллекции, если она передана
            if (scenarios != null)
            {
                var scenario = scenarios.FirstOrDefault(s => s.Name == scenarioName);
                if (scenario != null)
                {
                    scenarios.Remove(scenario);
                }
            }

            // Удаляем файл
            var filePath = Path.Combine(_scenariosFolder, $"{scenarioName}.json");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch
                {
                    // можно логировать ошибку
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Сохраняет коллекцию сценариев
        /// </summary>
        public void SaveScenarios(ObservableCollection<ScenarioModel> scenarios)
        {
            foreach (var s in scenarios)
                SaveScenario(s);
        }
    }
}
