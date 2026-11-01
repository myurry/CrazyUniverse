using CrazyUniverse.Core.Interfaces;
using CrazyUniverse.Core.Models;
using CrazyUniverse.WpfApp.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CrazyUniverse.WpfApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel? ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void CmbAnimalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content is string animalType && ViewModel != null)
                {
                    ViewModel.NewAnimalType = animalType;
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.NewAnimalName = TxtName.Text;

                if (ViewModel.AddCommand.CanExecute(null))
                {
                    ViewModel.AddCommand.Execute(null);
                    TxtName.Text = string.Empty;
                }
            }
        }

        private void LstAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listView && ViewModel != null)
            {
                ViewModel.SelectedAnimal = listView.SelectedItem as Animal;
            }
        }

        private void BtnMakeSound_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.MakeSoundCommand != null && ViewModel.MakeSoundCommand.CanExecute(null))
            {
                ViewModel.MakeSoundCommand.Execute(null);
            }
        }

        private async void OnCrazyActionClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.CrazyActionCommand != null && ViewModel.CrazyActionCommand.CanExecute(null))
            {
                ViewModel.CrazyActionCommand.Execute(null);
            }
            await Task.CompletedTask;
        }

        private void BtnCrazyAction_Click(object sender, RoutedEventArgs e)
        {
            OnCrazyActionClick(sender, e);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.RemoveCommand != null && ViewModel.RemoveCommand.CanExecute(null))
            {
                ViewModel.RemoveCommand.Execute(null);
            }
        }

        private void BtnTriggerAbility_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SelectedAnimal == null) return;

            var animal = ViewModel.SelectedAnimal;
            bool actionExecuted = false;

            Action<string> logger = message =>
            {
                ViewModel.LogText += message + "\n";
            };

            if (animal is IFlyable flyable)
            {
                flyable.Fly(logger);
                actionExecuted = true;
            }

            if (animal is IClimbable climbable)
            {
                climbable.Climb(logger);
                actionExecuted = true;
            }

            if (animal is ISwimmable swimmable)
            {
                swimmable.Swim(logger);
                actionExecuted = true;
            }

            if (!actionExecuted)
            {
                ViewModel.LogText += $"[{animal.Name}] has no special interface abilities.\n";
            }
        }

        private void AnimalTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource is TabControl && sender is TabControl tabControl)
            {
                if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Tag is string tagValue)
                {
                    if (ViewModel != null)
                    {
                        ViewModel.SelectedAnimalType = tagValue;
                    }
                }
            }
        }

        // --- Vol.3 Simulation UI Event Handlers ---

        private void CmbWorkload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Tag?.ToString() is string tagStr && int.TryParse(tagStr, out int workload) && ViewModel != null)
                {
                    ViewModel.SelectedWorkloadSize = workload;
                }
            }
        }

        private async void BtnRunSequential_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.RunSequentialCommand != null && ViewModel.RunSequentialCommand.CanExecute(null))
            {
                ViewModel.RunSequentialCommand.Execute(null);
            }
            await Task.CompletedTask;
        }

        private async void BtnRunParallel_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.RunParallelCommand != null && ViewModel.RunParallelCommand.CanExecute(null))
            {
                ViewModel.RunParallelCommand.Execute(null);
            }
            await Task.CompletedTask;
        }

        private void BtnCancelSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.CancelSimulationCommand != null && ViewModel.CancelSimulationCommand.CanExecute(null))
            {
                ViewModel.CancelSimulationCommand.Execute(null);
            }
        }
    }
}