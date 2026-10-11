using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CrazyUniverse.Core.Interfaces;
using CrazyUniverse.Core.Models;

namespace CrazyUniverse.WpfApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Animal> _animals = new();

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            _animals = new ObservableCollection<Animal>
            {
                new Cow("Bessie", 80),
                new Horse("Shadowfax", 90),
                new Monkey("George", 75),
                new Bird("Skye", 60)
            };

            LstAnimals.ItemsSource = _animals;
            Log("Application started. Welcome to Crazy Universe!");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TxtName.Text;

                // Validate and parse energy level securely
                if (!int.TryParse(TxtEnergy.Text, out int energy))
                {
                    throw new FormatException("Energy level must be a valid integer number.");
                }

                Animal newAnimal;
                var selectedItem = CmbAnimalType.SelectedItem as ComboBoxItem;
                string selectedType = selectedItem?.Content?.ToString() ?? "Cow";

                switch (selectedType)
                {
                    case "Cow":
                        newAnimal = new Cow(name, energy);
                        break;
                    case "Horse":
                        newAnimal = new Horse(name, energy);
                        break;
                    case "Monkey":
                        newAnimal = new Monkey(name, energy);
                        break;
                    case "Bird":
                        newAnimal = new Bird(name, energy);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown animal type selected.");
                }

                _animals.Add(newAnimal);
                LstAnimals.SelectedItem = newAnimal;
                Log($"Successfully added new {selectedType}: {newAnimal.Name}");

                // Clear input fields
                TxtName.Clear();
                TxtEnergy.Text = "50";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Validation Error: {ex.Message}", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                Log($"Failed to add animal: {ex.Message}");
            }
        }

        private void LstAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                // Check interface capabilities using 'is' operator for UI feedback
                if (selectedAnimal is IClimbable)
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
                // Polymorphic call
                string soundMsg = selectedAnimal.MakeSound();
                Log(soundMsg);
            }
            else
            {
                MessageBox.Show("Please select an animal first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCrazyAction_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                // Polymorphic CrazyAction call that modifies state and returns a message
                string crazyMsg = selectedAnimal.CrazyAction();
                Log($"[CRAZY ACTION]: {crazyMsg}");

                // Refresh UI binding by resetting item source view
                LstAnimals.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select an animal first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnTriggerAbility_Click(object sender, RoutedEventArgs e)
        {
            // Using 'as' operator to invoke interface methods safely
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                if (selectedAnimal is IClimbable climber)
                {
                    Log($"[INTERFACE]: {climber.Climb()}");
                }
                else if (selectedAnimal is IFlyable flyer)
                {
                    Log($"[INTERFACE]: {flyer.Fly()}");
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (LstAnimals.SelectedItem is Animal selectedAnimal)
            {
                _animals.Remove(selectedAnimal);
                Log($"Removed {selectedAnimal.Name} from sanctuary.");
            }
        }

        private void Log(string message)
        {
            TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            TxtLog.ScrollToEnd();
        }
    }
}