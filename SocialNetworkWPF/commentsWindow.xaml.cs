using DTO;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for commentsWindow.xaml
    /// </summary>
    public partial class commentsWindow : Window
    {
        ObjectId currUserId;
        ObjectId currPostId;
        List<Comment> comments = new List<Comment>();
        public commentsWindow(ObjectId _currUserId, ObjectId _currPostId)
        {
            currUserId = _currUserId;
            currPostId = _currPostId;
            InitializeComponent();
            printComments();
        }

        public void createComment(Comment comment)
        {

            Canvas canvas = new Canvas();
            canvas.Height = 200;
            TextBlock textBlockUserInfo = new TextBlock();
            textBlockUserInfo.Text = DataControl.GetUser(comment.userId).FirstName + DataControl.GetUser(comment.userId).SecondName;
            TextBlock textBlockComment = new TextBlock();
            textBlockComment.Tag = comment.Id;
            textBlockComment.Width = 400;
            //textBlockComment.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#545d6a"));
            textBlockComment.TextWrapping = TextWrapping.Wrap;
            textBlockComment.Text = comment.Text;
            textBlockComment.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#003264"));
            canvas.Children.Add(textBlockUserInfo);
            Canvas.SetTop(textBlockUserInfo, 1);
            canvas.Children.Add(textBlockComment);
            Canvas.SetTop(textBlockComment, 15);
            AllCommentsStack.Children.Add(canvas);
        }
        public void printComments()
        {
            AllCommentsStack.Children.Clear();
            comments = DataControl.GetPostComments(currPostId);
            foreach (var comment in comments)
            {
                createComment(comment);
            }
        }

        private void commentBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Comment comment = new Comment()
            {
                Text = commentText.Text,
                insertTime = DateTime.Now,
                userId = currUserId
            };
            DataControl.AddComment(comment, currPostId);
            printComments();
        }
        void commentsWindow_Closing(object sender, CancelEventArgs e)
        {
            AllCommentsStack.Children.Clear();
            comments.Clear();
        }
    }
}
