using CrazyUniverse.Core.Models;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;

namespace CrazyUniverse.Core.Services
{
    public class SanctuaryManager
    {
        private readonly List<Animal> _animals = new();

        // 1. Backing collection for Core activity logs
        private readonly List<string> _activityLogs = new();

        public IReadOnlyCollection<Animal> Animals => _animals.AsReadOnly();
        public IReadOnlyCollection<string> ActivityLogs => _activityLogs.AsReadOnly();

        // 2. Event to send logs back to WPF
        public event EventHandler<string>? LogMessageTriggered;

        public SanctuaryManager()
        {
            _animals.Add(new Cow(GetRandomName("CowNames"), 80));
            _animals.Add(new Horse(GetRandomName("HorseNames"), 90));
            _animals.Add(new Monkey(GetRandomName("MonkeyNames"), 75));
            _animals.Add(new Bird(GetRandomName("BirdNames"), 60));

            AddLog("Application started. Connected to Core Sanctuary Manager.");
        }

        // 3. The missing AddLog method!
        public void AddLog(string message)
        {
            string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _activityLogs.Add(formattedMessage);
            LogMessageTriggered?.Invoke(this, formattedMessage);
        }

        public static string GetRandomName(string key)
        {
            var random = new Random();
            string namesList = AnimalNames.ResourceManager.GetString(key) ?? "Unnamed Creature";
            string[] names = namesList.Split('|');
            return names[random.Next(names.Length)];
        }

        public void AddAnimal(Animal animal)
        {
            _animals.Add(animal);
            AddLog($"Successfully added new {animal.GetType().Name}: {animal.Name}");
        }

        public void RemoveAnimal(Animal animal)
        {
            _animals.Remove(animal);
            AddLog($"Removed {animal.Name} from sanctuary.");
        }

        public async Task<string> ExecuteCrazyActionAsync(Animal animal)
        {
            // Check if the animal has enough energy to perform a crazy action
            if (animal.EnergyLevel <= 0)
            {
                string depletedMessage = $"{animal.Name} is completely out of energy (Energy: {animal.EnergyLevel}) and cannot perform a crazy action!";
                AddLog($"[WARNING]: {depletedMessage}");
                return depletedMessage;
            }

            string message = animal.CrazyAction();

            if (animal is Horse)
            {
                for (int i = 1; i <= 100; i++)
                {
                    _animals.Add(new Horse($"{animal.Name}_Clone{i}", animal.EnergyLevel));
                }
                message += " [CHAOS]: 100 clones have flooded the sanctuary!";
            }
            else if (animal is Monkey)
            {
                _ = Task.Run(async () =>
                {
                    for (int i = 5; i >= 1; i--)
                    {
                        await Task.Delay(1000);
                        AddLog($"[SYSTEM WARNING]: Deleting activity logs in {i}...");
                    }

                    await Task.Delay(1000);

                    _activityLogs.Clear();
                    AddLog($"[CRITICAL]: {animal.Name} successfully wiped all core activity logs clean!");
                });
            }
            else if (animal is Bird)
            {
                _ = Task.Run(async () =>
                {
                    for (int i = 3; i >= 1; i--)
                    {
                        await Task.Delay(1000);
                        AddLog($"[WARNING]: {animal.Name} self-destruct in {i}...");
                    }

                    await Task.Delay(1000);

                    _animals.Remove(animal);
                    AddLog($"[BOOM]: {animal.Name} has self-destructed and vanished into thin air!");
                });
            }

            return message;
        }
    }
}