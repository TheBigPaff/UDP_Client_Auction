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

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for SignWindow.xaml
    /// </summary>
    public partial class SignWindow : Window
    {
        public SignWindow()
        {
            InitializeComponent();

            backBtn.Visibility = Visibility.Collapsed;
            contentArea.Content = new LogInControl();
        }
        public SignWindow(double _leftPosition, double _topPosition)
        {
            InitializeComponent();

            this.Left = _leftPosition;
            this.Top = _topPosition;

            backBtn.Visibility = Visibility.Collapsed;
            contentArea.Content = new LogInControl();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void EnterApp(string _picPath)
        {
            new MainWindow(this.Left, this.Top) { ImageSource = new Uri(_picPath, UriKind.RelativeOrAbsolute) }.Show();
            this.Close();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if(proPicImgPopUp.Visibility == Visibility.Visible)
            {
                proPicImgPopUp.Hide();
            }

            contentArea.Content = new LogInControl();
            backBtn.Visibility = Visibility.Collapsed;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        internal void ShowSignUp()
        {
            backBtn.Visibility = Visibility.Visible;
            contentArea.Content = new SignUpControl();
        }

        internal void ShowForgotPassword()
        {
            backBtn.Visibility = Visibility.Visible;
            contentArea.Content = new ForgotPasswordControl();
        }
        internal void ShowLogIn()
        {
            backBtn.Visibility = Visibility.Collapsed;
            contentArea.Content = new LogInControl();
        }
    }
}
