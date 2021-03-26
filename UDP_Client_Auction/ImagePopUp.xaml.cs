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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UDP_Client_Auction
{
    /// <summary>
    /// Interaction logic for ImagePopUp.xaml
    /// </summary>
    public partial class ImagePopUp : UserControl
    {
        public int ImgMaxHeight
        {
            get { return (int)GetValue(ImgMaxHeightProperty); }
            set { SetValue(ImgMaxHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImgMaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgMaxHeightProperty =
            DependencyProperty.Register("ImgMaxHeight", typeof(int), typeof(ImagePopUp), new PropertyMetadata(600)); // PropertyMetadata is the default value

        public int ImgMaxWidth
        {
            get { return (int)GetValue(ImgMaxWidthProperty); }
            set { SetValue(ImgMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImgMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgMaxWidthProperty =
            DependencyProperty.Register("ImgMaxWidth", typeof(int), typeof(ImagePopUp), new PropertyMetadata(850));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(ImagePopUp));

        public ImagePopUp()
        {
            InitializeComponent();
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }


        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
        }
    }
}

