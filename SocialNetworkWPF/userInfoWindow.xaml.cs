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
    /// Interaction logic for userInfoWindow.xaml
    /// </summary>
    public partial class userInfoWindow : Window
    {
        ObjectId currUserId;
        ObjectId userFromStackId;
        MainWindow mainwindow = null;
        public userInfoWindow(ObjectId _currUser, ObjectId _userFromStack, Window callingwindow)
        {
            mainwindow = callingwindow as MainWindow;
            currUserId = _currUser;
            userFromStackId = _userFromStack;
            InitializeComponent();
            LoadUserInfoAsync();
            
        }
        private async void LoadUserInfoAsync()
        {
            var posts = DataControl.GetUserPosts(userFromStackId);
            User currUser = DataControl.GetUser(userFromStackId);
            currUserFirstName.Text = currUser.FirstName;
            currUserSecondName.Text = currUser.SecondName;

            foreach (var post in posts)
            {
                CreatePosts(post);
            }

            await FollowUnfollow();


            string pathLength = await DataControl.GetPathLength(Convert.ToString(currUserId), Convert.ToString(userFromStackId));

            GraphNumber.Text += pathLength + " handshakes";
        }
        public void CreatePosts(Post post)
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
            TextBlock numberOfLikes = new TextBlock();
            numberOfLikes.Height = 30;
            numberOfLikes.Width = 32;
            numberOfLikes.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF5E7"));
            numberOfLikes.FontSize = 10;
            canvas.Children.Add(numberOfLikes);
            Canvas.SetRight(numberOfLikes, 0);
            Canvas.SetTop(numberOfLikes, 70);
            numberOfLikes.Text = Convert.ToString(DataControl.GetPostById(ObjectId.Parse(Convert.ToString(textBlockPost.Tag))).UserLikesId.Count());
            likebtn.MouseDown += (sender, e) =>
            {
                DataControl.LikePost(currUserId, ObjectId.Parse(Convert.ToString(textBlockPost.Tag)));
                numberOfLikes.Text = Convert.ToString(DataControl.GetPostById(ObjectId.Parse(Convert.ToString(textBlockPost.Tag))).UserLikesId.Count());
            };
            commentbtn.MouseDown += (sender, e) =>
            {
                commentsWindow postCommentsWindow = new commentsWindow(currUserId, post.Id);
                postCommentsWindow.Show();
            };
            canvas.Children.Add(scrollViewerPost);
            Canvas.SetTop(scrollViewerPost, 5);
            AllPostStack.Children.Add(canvas);

        }

        public async Task FollowUnfollow() 
        {
            if (!DataControl.IsFollowed(currUserId, userFromStackId))
            {
                Button followbtn = new Button();
                followbtn.Height = 30;
                followbtn.Width = 70;
                followbtn.Content = "Follow";
                followbtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF5E7"));
                followbtn.Click += async (sender, e) => {
                    DataControl.FollowUser(currUserId, userFromStackId);
                    await DataControl.FollowUserGraph(Convert.ToString(currUserId), Convert.ToString(userFromStackId));
                    await FollowUnfollow();
                };
                stackUserInfo.Children.Add(followbtn);
                Canvas.SetTop(followbtn, 50);
                //this.mainwindow.followingBlock.Text = "Following " + DataControl.GetUser(currUserId).FollowingsId.Count(); ///////////////////

            }
            else
            {
                Button unfollowbtn = new Button();
                unfollowbtn.Height = 30;
                unfollowbtn.Width = 70;
                unfollowbtn.Content = "UnFollow";
                unfollowbtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF5E7"));
                stackUserInfo.Children.Add(unfollowbtn);
                unfollowbtn.Click += (sender, e) => {
                    DataControl.UnFollowUser(currUserId, userFromStackId);
                    DataControl.UnFollowUserGraph(Convert.ToString(currUserId), Convert.ToString(userFromStackId));
                    FollowUnfollow();
                };
                Canvas.SetTop(unfollowbtn, 50);
            }
        }

    }
}
