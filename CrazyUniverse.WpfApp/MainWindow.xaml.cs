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
                    // Устанавливаем тип только для СОЗДАНИЯ нового животного, 
                    // не трогая текущую отображаемую вкладку репозитория!
                    ViewModel.NewAnimalType = animalType;
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                // 1. Берем текст из TextBox TxtName
                ViewModel.NewAnimalName = TxtName.Text;

                // 2. Если имя заполнено, вызываем команду добавления
                if (ViewModel.AddCommand.CanExecute(null))
                {
                    ViewModel.AddCommand.Execute(null);

                    // 3. Очищаем текстовое поле в UI после успешного добавления
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

            // Define the logger delegate to update the UI log
            Action<string> logger = message =>
            {
                ViewModel.LogText += message + "\n";
            };

            // Check and trigger IFlyable
            if (animal is IFlyable flyable)
            {
                flyable.Fly(logger);
                actionExecuted = true;
            }

            // Check and trigger IClimbable
            if (animal is IClimbable climbable)
            {
                climbable.Climb(logger);
                actionExecuted = true;
            }

            // Check and trigger ISwimmable
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
    }
}