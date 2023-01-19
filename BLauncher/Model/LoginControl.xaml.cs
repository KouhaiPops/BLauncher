using BLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLauncher.Model
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
            // Must set data context in code behind to pass password box instance
            DataContextChanged += (_, _) =>
            {
                if (DataContext is MainWindowViewModel context)
                {
                    context.LoginVM.PasswordBox = passwordBox;
                }
            };
        }
    }
}
