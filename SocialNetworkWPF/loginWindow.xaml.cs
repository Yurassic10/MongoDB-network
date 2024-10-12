using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SocialNetworkWPF
{
    /// <summary>
    /// Interaction logic for loginWindow.xaml
    /// </summary>
    public partial class loginWindow : Window
    {
        string enteredEMail;
        string enteredPassword;
        public loginWindow()
        {
            InitializeComponent();

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            enteredEMail = txtEmail.Text;
            enteredPassword = txtPassword.Text;

            if (DataControl.FindUserByEmailAndPassword(enteredEMail, enteredPassword))
            {
                User user = DataControl.GetUserByEmailAndPass(enteredEMail, enteredPassword);
                MainWindow mainwindow = new MainWindow(Convert.ToString(user.Id));
                mainwindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong email or password");
            }
        }
    }
}
