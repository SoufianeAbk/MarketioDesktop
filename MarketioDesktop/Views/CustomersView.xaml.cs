using MarketioDesktop.ViewModels;
using MarketioDesktop.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace MarketioDesktop.Views
{
    public partial class CustomersView : UserControl
    {
        public CustomersView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CustomersViewModel vm) return;

            vm.LoadCustomersCommand.Execute(null);

            vm.CreateCustomerRequested += (_, _) =>
            {
                var dialog = new CustomerDialog { Owner = Window.GetWindow(this) };
                if (dialog.ShowDialog() == true)
                    _ = vm.SubmitCreateCustomerAsync(
                        dialog.FirstName, dialog.LastName,
                        dialog.Email, dialog.PhoneNumber,
                        dialog.Address, dialog.IsActiveChecked);
            };

            vm.EditCustomerRequested += (_, customer) =>
            {
                var dialog = new CustomerDialog(customer) { Owner = Window.GetWindow(this) };
                if (dialog.ShowDialog() == true)
                    _ = vm.SubmitUpdateCustomerAsync(
                        (string)customer.Id,
                        dialog.FirstName, dialog.LastName,
                        dialog.Email, dialog.PhoneNumber,
                        dialog.Address, dialog.IsActiveChecked);
            };
        }
    }
}