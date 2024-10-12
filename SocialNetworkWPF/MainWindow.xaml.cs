using System;
//using DAL;
using DTO;
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
using MongoDB.Bson;

namespace SocialNetworkWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int j = 0;
        ObjectId currentUserId;

        public MainWindow(string loggedUserId)
        {
            currentUserId = ObjectId.Parse(loggedUserId);
            InitializeComponent();
            var posts = DataControl.GetUserPosts(currentUserId);
            User currUser = DataControl.GetUser(currentUserId);
            currUserFirstName.Text = currUser.FirstName;
            currUserSecondName.Text = currUser.SecondName;
            foreach (var post in posts)
            {
                createPost(post);
            }
            var currUserInterest = DataControl.GetUserInterests(currentUserId);
            TextBlock interestBox1 = new TextBlock();
            interestBox1.Width = 160;
            interestBox1.Text = "Interest:";
            userInterestsStack.Children.Add(interestBox1);
            foreach (var interest in currUserInterest)
            {
                TextBlock interestBox = new TextBlock();
                interestBox.Width = 160;
                interestBox.Text = interest;
                userInterestsStack.Children.Add(interestBox);
            }
            printUsers();

            followersBlock.Text = "Followers " + DataControl.GetUser(currentUserId).FollowersId.Count();
            followingBlock.Text = "Following " + DataControl.GetUser(currentUserId).FollowingsId.Count();
        }

        public void printPosts()
        {
            AllPostStack.Children.Clear();

            var posts = DataControl.GetUserPosts(currentUserId);
            foreach (var post in posts)
            {
                createPost(post);
            }
        }

        public void Message(TextBlock textblock)
        {
            MessageBox.Show("Close?", Convert.ToString(textblock.Tag));
        }


        public void createPost(Post post)
        {

            Canvas likebtn = new Canvas();
            likebtn.Height = 30;
            likebtn.Width = 32;
            var brushLike = new ImageBrush();
            brushLike.ImageSource = new BitmapImage(new Uri("Images/heart.png", UriKind.Relative));
            likebtn.Background = brushLike;



            Canvas commentbtn = new Canvas();
            commentbtn.Height = 30;
            commentbtn.Width = 32;
            var brushComment = new ImageBrush();
            brushComment.ImageSource = new BitmapImage(new Uri("Images/comments.png", UriKind.Relative));
            commentbtn.Background = brushComment;
            commentbtn.MouseDown += (sender, e) =>
            {
                commentsWindow postCommentsWindow = new commentsWindow(currentUserId, post.Id);
                postCommentsWindow.Show();
            };


            Canvas canvas = new Canvas();
            canvas.Height = 200;
            canvas.Children.Add(commentbtn);
            Canvas.SetRight(commentbtn, 5);
            Canvas.SetTop(commentbtn, 1);
            canvas.Children.Add(likebtn);
            Canvas.SetRight(likebtn, 5);
            Canvas.SetTop(likebtn, 35);

            TextBlock textBlockPost = new TextBlock();
            textBlockPost.Tag = post.Id;
            textBlockPost.Width = 420;
            textBlockPost.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#545d6a"));
            textBlockPost.TextWrapping = TextWrapping.Wrap;
            textBlockPost.Text = post.Text;
            textBlockPost.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF5E7"));
            ScrollViewer scrollViewerPost = new ScrollViewer();
            scrollViewerPost.Height = 200;
            scrollViewerPost.Width = 430;
            scrollViewerPost.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerPost.Content = textBlockPost;

            likebtn.MouseDown += (sender, e) =>
            {
                DataControl.LikePost(currentUserId, ObjectId.Parse(Convert.ToString(textBlockPost.Tag)));
            };
            canvas.Children.Add(scrollViewerPost);
            Canvas.SetTop(scrollViewerPost, 5);
            AllPostStack.Children.Add(canvas);


        }

        private void addPost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            writePostWindow writePost = new writePostWindow(this, Convert.ToString(currentUserId));
            writePost.Show();
        }

        public void printUsers()
        {
            List<User> otherUsers = DataControl.GetOtherUsers(currentUserId);
            foreach (User user in otherUsers)
            {
                StackPanel userInformation = new StackPanel();
                userInformation.Orientation = Orientation.Horizontal;
                TextBlock userInfoBlock = new TextBlock();
                userInfoBlock.Width = 100;
                Button followButton = new Button();
                followButton.Height = 20;
                followButton.Width = 20;
                string userInfoStr = DataControl.GetUser(user.Id).FirstName + " " + DataControl.GetUser(user.Id).SecondName;
                userInfoBlock.Text = userInfoStr;
                followButton.Click += (sender, e) =>
                {
                    OpenUser(user.Id);
                };
                userInformation.Children.Add(userInfoBlock);
                userInformation.Children.Add(followButton);
                otherUsersListBox.Items.Add(userInformation);
            }
        }

        public void OpenUser(ObjectId userId)
        {
            userInfoWindow userInfoWindow = new userInfoWindow(currentUserId, userId, this);
            userInfoWindow.Show();
        }
    }
}
