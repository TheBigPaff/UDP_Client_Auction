using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _WinForms = System.Windows.Forms;

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for SignUpControl.xaml
    /// </summary>
    public partial class SignUpControl : UserControl
    {
        string proPicPath  = "";

        SignWindow parentWindow;
        public SignUpControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
            if (signWindow != null)
            {
                parentWindow = signWindow;
            }


            proPicImg.Source = new BitmapImage(new Uri(@"/Assets/user_icon.png", UriKind.RelativeOrAbsolute));
            parentWindow.proPicImgPopUp.Source = "/Assets/user_icon.png";
        }


        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) ||  string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Some fields are empty");
                return;
            }
            else if (DB.DoesUsernameExist(tbUsername.Text))
            {
                MessageBox.Show("Username already exists");
                return;
            }
            else if(pbPassword.Password.Contains(" "))
            {
                MessageBox.Show("Password cannot contain spaces");
                return;
            }
            else if (tbUsername.Text.Length > 32 || pbPassword.Password.Length > 32)
            {
                MessageBox.Show("Usernames and password can't be longer than 32 characters");
                return;
            }
            else if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                SystemSounds.Exclamation.Play();
                MessageBoxResult _dialogResult = MessageBox.Show("If you don't insert an email then you won't be able to access your account in case you forget your password. Confirm empty email?", "No email", MessageBoxButton.YesNo);
                if (_dialogResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(proPicPath)) proPicPath = "/Assets/user_icon.png";

            string hashedPassword = Helper.HashPassword(pbPassword.Password);
            User user = new User()
            {
                Username = tbUsername.Text,
                Email = tbEmail.Text,
                Password = hashedPassword,
                ProPicPath = proPicPath,
            };

            Session.LoggedUser = user;

            if (DB.RegisterUser(user))
            {
                MessageBox.Show("User registered successfully!");
                parentWindow.EnterApp(user.ProPicPath);
            }

        }

        private void btnOpenImageDialog_Click(object sender, RoutedEventArgs e)
        {
            using (_WinForms.OpenFileDialog openFileDialog = new _WinForms.OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == _WinForms.DialogResult.OK)
                {
                    //Get the path of specified file
                    proPicPath = openFileDialog.FileName;
                    proPicImg.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                    parentWindow.proPicImgPopUp.Source = openFileDialog.FileName;
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void proPicImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            parentWindow.proPicImgPopUp.Show();
        }
    }
}
