using DTO;
using MongoDB.Bson;
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

namespace SocialNetworkWPF
{
    /// <summary>
    /// Interaction logic for writePostWindow.xaml
    /// </summary>
    public partial class writePostWindow : Window
    {
        MainWindow mainwindow = null;
        string userId;
        public writePostWindow(Window callingwindow, string currentUserId)
        {
            userId = currentUserId;
            mainwindow = callingwindow as MainWindow;
            InitializeComponent();
        }
        private void postButton_Click(object sender, RoutedEventArgs e)
        {
            Post post = new Post()
            {
                Text = postText.Text,
                userId = ObjectId.Parse(userId),
                insertTime = DateTime.Now,
                CommentsId = new List<ObjectId>(),
                UserLikesId = new List<ObjectId>()
            };
            DataControl.AddPost(post, ObjectId.Parse(userId));

            this.mainwindow.createPost(post);
            this.mainwindow.printPosts();
            Close();
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Close?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
