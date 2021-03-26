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
    /// Interaction logic for UserInfoControl.xaml
    /// </summary>
    public partial class UserInfoControl : UserControl
    {
        bool isEditing = false;
        public string proPicPath = Session.LoggedUser.ProPicPath;

        public UserInfoControl()
        {
            InitializeComponent();
        }

        private void userInfoControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            nameTb.Text = Session.LoggedUser.Username;
            tbUsername.IsReadOnly = true;
            tbEmail.IsReadOnly = true;
            pbPassword.Focusable = false;
            pbPassword.IsHitTestVisible = false;
            btnChooseImage.Visibility = Visibility.Collapsed;

            tbUsername.Text = Session.LoggedUser.Username;
            tbEmail.Text = Session.LoggedUser.Email;
            pbPassword.Password = Session.LoggedUser.Password;
            userImg.Source = new BitmapImage(new Uri(Session.LoggedUser.ProPicPath, UriKind.RelativeOrAbsolute));
            UserImagePopup.Source = Session.LoggedUser.ProPicPath;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            InputBox.Visibility = Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // YesButton Clicked! Let's hide our InputBox and handle the input text.
            InputBox.Visibility = Visibility.Collapsed;


            if (!InputTextBox.Text.Equals(Session.LoggedUser.Username.ToUpper()))
            {
                MessageBox.Show("Wrong input, your account has NOT been deleted.");
            }
            else
            {
                DB.DeleteUser(Session.LoggedUser.Id);

                MessageBox.Show("Account deleted successfully!");
                new SignWindow().Show();
                Window.GetWindow(this).Close();
            }

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            InputBox.Visibility = Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
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
                    userImg.Source = new BitmapImage(new Uri(proPicPath, UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(pbPassword.Password))
                {
                    MessageBox.Show("Some fields are empty");
                    return;
                }
                else if (pbPassword.Password.Contains(" "))
                {
                    MessageBox.Show("Password cannot contain spaces");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(proPicPath))
                {
                    MessageBox.Show("You didn't choose a picture");
                    return;
                }
                else if(tbUsername.Text != Session.LoggedUser.Username && DB.GetUserId(tbUsername.Text) != -1)
                {
                    MessageBox.Show("A user with that name already exists.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(tbEmail.Text))
                {
                    SystemSounds.Exclamation.Play();
                    MessageBoxResult dialogResult = MessageBox.Show("If you don't insert an email then you won't be able to access your account in case you forget your password. Confirm empty email?", "No email", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                User user = new User()
                {
                    Id = Session.LoggedUser.Id,
                    Username = tbUsername.Text,
                    Email = tbEmail.Text,
                    Password = Helper.HashPassword(pbPassword.Password),
                    ProPicPath = proPicPath
                };

                bool updatePassword = pbPassword.Password != Session.LoggedUser.Password;

                DB.UpdateUser(user, updatePassword);
                Session.LoggedUser = user;

                MessageBox.Show("User updated successfully!");

                Helper.GetAncestorOfType<MainWindow>(this).usernameBtn.Content=user.Username;

                MainWindow mainWindow = Helper.GetAncestorOfType<MainWindow>(this);
                if(mainWindow != null)
                {
                    mainWindow.ImageSource = new Uri(proPicPath, UriKind.RelativeOrAbsolute);
                }

                isEditing = false;
                btnSignUp.Content = "EDIT INFO";

                LoadUserInfo();
            }
            else
            {
                tbUsername.IsReadOnly = false;
                tbEmail.IsReadOnly = false;
                pbPassword.Focusable = true;
                pbPassword.IsHitTestVisible = true;
                btnChooseImage.Visibility = Visibility.Visible;

                isEditing = true;
                btnSignUp.Content = "CONFIRM";
            }
        }


        private void showBMIInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            BMIImagePopup.Show();
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && btnSignUp.Content.ToString() == "CONFIRM")
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void userImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                UserImagePopup.Show();
            }
        }
    }
}
