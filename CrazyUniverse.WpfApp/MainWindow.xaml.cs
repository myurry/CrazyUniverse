using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CrazyUniverse.Core.Interfaces;
using CrazyUniverse.Core.Models;
using CrazyUniverse.Core.Services;

namespace CrazyUniverse.WpfApp
{
    public partial class MainWindow : Window
    {
        private SanctuaryManager _sanctuaryManager;
        private ObservableCollection<Animal> _animalsViewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            _sanctuaryManager = new SanctuaryManager();

            TxtLog.Clear();
            foreach (var log in _sanctuaryManager.ActivityLogs)
            {
                TxtLog.AppendText($"{log}{Environment.NewLine}");
            }
            TxtLog.ScrollToEnd();

            _animalsViewModel = new ObservableCollection<Animal>(_sanctuaryManager.Animals);
            LstAnimals.ItemsSource = _animalsViewModel;

            _sanctuaryManager.LogMessageTriggered += (s, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (message.Contains("successfully wiped all core activity logs clean"))
                    {
                        TxtLog.Clear();
                    }

                    TxtLog.AppendText($"{message}{Environment.NewLine}");
                    TxtLog.ScrollToEnd();

                    if (message.Contains("has self-destructed"))
                    {
                        RefreshView();
                    }
                });
            };

            // Set initial name preview based on default dropdown selection
            UpdateNamePreview();
        }


        private void UpdateNamePreview()
        {
            // Ensure TxtName is initialized before trying to write to it
            if (TxtName == null || CmbAnimalType?.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            string selectedType = selectedItem.Content?.ToString() ?? "Cow";

            // Fetch random name from Core based on type
            string randomName = selectedType switch
            {
                "Cow" => SanctuaryManager.GetRandomName("CowNames"),
                "Horse" => SanctuaryManager.GetRandomName("HorseNames"),
                "Monkey" => SanctuaryManager.GetRandomName("MonkeyNames"),
                "Bird" => SanctuaryManager.GetRandomName("BirdNames"),
                _ => SanctuaryManager.GetRandomName("AllNames")
            };

            TxtName.Text = randomName;
        }

        private void CmbAnimalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNamePreview();
        }

        private void RefreshView()
        {
            _animalsViewModel.Clear();
            foreach (var animal in _sanctuaryManager.Animals)
            {
                _animalsViewModel.Add(animal);
            }
            LstAnimals.Items.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = CmbAnimalType.SelectedItem as ComboBoxItem;
                string selectedType = selectedItem?.Content?.ToString() ?? "Cow";

                // If name input is empty, randomly fetch one from AnimalNames.resx based on type!
                string name = TxtName.Text.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = selectedType switch
                    {
                        "Cow" => SanctuaryManager.GetRandomName("CowNames"),
                        "Horse" => SanctuaryManager.GetRandomName("HorseNames"),
                        "Monkey" => SanctuaryManager.GetRandomName("MonkeyNames"),
                        "Bird" => SanctuaryManager.GetRandomName("BirdNames"),
                        _ => SanctuaryManager.GetRandomName("AllNames")
                    };
                }

                if (!int.TryParse(TxtEnergy.Text, out int energy))
                {
                    throw new FormatException("Energy level must be a valid integer number.");
                }

                Animal newAnimal = selectedType switch
                {
                    "Cow" => new Cow(name, energy),
                    "Horse" => new Horse(name, energy),
                    "Monkey" => new Monkey(name, energy),
                    "Bird" => new Bird(name, energy),
                    _ => throw new InvalidOperationException("Unknown animal type selected.")
                };

                _sanctuaryManager.AddAnimal(newAnimal);
                RefreshView();
                LstAnimals.SelectedItem = newAnimal;

                TxtName.Clear();
                TxtEnergy.Text = "50";

                UpdateNamePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Validation Error: {ex.Message}", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                _sanctuaryManager.AddLog($"Failed to add animal: {ex.Message}");
            }
        }

        private void LstAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                if (selectedAnimal is IClimbable && selectedAnimal is ISwimmable)
                {
                    TxtAbilityStatus.Text = $"{selectedAnimal.Name} is multi-talented (IClimbable & ISwimmable)!";
                    BtnTriggerAbility.IsEnabled = true;
                }
                else if (selectedAnimal is IClimbable)
                {
                    TxtAbilityStatus.Text = $"{selectedAnimal.Name} is IClimbable (can scale heights)!";
                    BtnTriggerAbility.IsEnabled = true;
                }
                else if (selectedAnimal is IFlyable)
                {
                    TxtAbilityStatus.Text = $"{selectedAnimal.Name} is IFlyable (can soar through the air)!";
                    BtnTriggerAbility.IsEnabled = true;
                }
                else
                {
                    TxtAbilityStatus.Text = $"{selectedAnimal.Name} has no special interface capabilities.";
                    BtnTriggerAbility.IsEnabled = false;
                }
            }
        }

        private void BtnMakeSound_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                _sanctuaryManager.AddLog(selectedAnimal.MakeSound());
            }
            else
            {
                MessageBox.Show("Please select an animal first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnCrazyAction_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                string crazyMsg = await _sanctuaryManager.ExecuteCrazyActionAsync(selectedAnimal);
                _sanctuaryManager.AddLog($"[CRAZY ACTION]: {crazyMsg}");

                RefreshView();
            }
            else
            {
                MessageBox.Show("Please select an animal first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnTriggerAbility_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                if (selectedAnimal is Monkey monkey)
                {
                    _sanctuaryManager.AddLog($"[INTERFACE]: {monkey.PerformSpecialAction()}");
                }
                else if (selectedAnimal is IClimbable climber)
                {
                    _sanctuaryManager.AddLog($"[INTERFACE]: {climber.Climb()}");
                }
                else if (selectedAnimal is IFlyable flyer)
                {
                    _sanctuaryManager.AddLog($"[INTERFACE]: {flyer.Fly()}");
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                _sanctuaryManager.RemoveAnimal(selectedAnimal);
                RefreshView();
            }
        }
    }
}