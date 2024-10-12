using System;
using DTO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
//using DAL;
using DAL;


namespace SocialNetworkWPF
{
    static public class DataControl
    {
        static MongoCRUD db;

        static List<Post> posts;
        static List<User> users;
        static List<Comment> comments = new List<Comment>();
        static DataControl()
        {
            db = new MongoCRUD("SocialNetwork");

        }

        static public List<User> GetUsers()
        {
            return users = db.ReadEntity<User>("Users");
        }
        static public User GetUser(ObjectId userId)
        {
            return db.GetEntityById<User>("Users", userId);
        }

        static public bool FindUserByEmailAndPassword(string email, string password)
        {
            int count = 0;
            users = GetUsers();
            var selectedUsers = from user in users
                                where user.Email == email
                                where user.Password == password
                                select user;
            if (selectedUsers.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static public User GetUserByEmailAndPass(string email, string password)
        {
            int count = 0;
            users = GetUsers();
            var selectedUsers = from user in users
                                where user.Email == email
                                where user.Password == password
                                select user;
            return selectedUsers.First();
        }
        static public List<Post> GetPosts()
        {
            return posts = db.ReadEntity<Post>("Posts");
        }
        static public IEnumerable<Post> GetUserPosts(ObjectId userId)
        {
            posts = GetPosts();
            User user = new User();
            user = db.GetEntityById<User>("Users", userId);
            var userPostsId = user.PostsId;
            List<Post> usersPosts = new List<Post>();

            var selectedPost = from post in posts
                               from userpostid in userPostsId
                               where post.Id == userpostid
                               select post;
            selectedPost = selectedPost.OrderByDescending(p => p.insertTime).ToList();
            return selectedPost;
        }

        static public void AddPost(Post post, ObjectId userId)
        {
            db.InsertEntity<Post>("Posts", post);
            var getUser = db.GetEntityById<User>("Users", userId);
            getUser.PostsId.Add(post.Id);
            db.UpsertEntity("Users", getUser.Id, getUser);
        }
        static public Post GetPostById(ObjectId postId)
        {
            return db.GetEntityById<Post>("Posts", postId);
        }

        static public void LikePost(ObjectId userId, ObjectId postId)
        {
            var getPost = db.GetEntityById<Post>("Posts", postId);

            if (getPost.UserLikesId.Contains(userId))
            {
                getPost.UserLikesId.Remove(userId);
                db.UpsertEntity("Posts", getPost.Id, getPost);
            }
            else
            {
                getPost.UserLikesId.Add(userId);
                db.UpsertEntity("Posts", getPost.Id, getPost);
            }
        }

        static public List<User> GetOtherUsers(ObjectId userId)
        {
            List<User> otherUsers = new List<User>();
            {
                foreach (User user in users)
                {
                    if (user.Id != userId)
                    {
                        otherUsers.Add(user);
                    }
                }
            }
            return otherUsers;
        }

        static public List<string> GetUserInterests(ObjectId userId)
        {
            User user = db.GetEntityById<User>("Users", userId);
            return user.Interests;
        }

        static public bool IsFollowed(ObjectId userCurr, ObjectId userStack)
        {
            User userFromStack = db.GetEntityById<User>("Users", userStack);

            if (userFromStack.FollowersId.Contains(userCurr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static public void FollowUser(ObjectId userId, ObjectId userFollowedId)
        {
            User userFollowed = db.GetEntityById<User>("Users", userFollowedId);
            userFollowed.FollowersId.Add(userId);
            db.UpsertEntity("Users", userFollowed.Id, userFollowed);
            User userrFollowing = db.GetEntityById<User>("Users", userId);
            userrFollowing.FollowingsId.Add(userFollowedId);
            db.UpsertEntity("Users", userrFollowing.Id, userrFollowing);
        }
        static public void UnFollowUser(ObjectId userId, ObjectId userFollowedId)
        {
            User userFollowed = db.GetEntityById<User>("Users", userFollowedId);
            userFollowed.FollowersId.Remove(userId);
            db.UpsertEntity("Users", userFollowed.Id, userFollowed);
            User userrFollowing = db.GetEntityById<User>("Users", userId);
            userrFollowing.FollowingsId.Remove(userFollowedId);
            db.UpsertEntity("Users", userrFollowing.Id, userrFollowing);
        }

        static public void AddComment(Comment comment, ObjectId postId)
        {
            db.InsertEntity("Comments", comment);
            var post = db.GetEntityById<Post>("Posts", postId);
            post.CommentsId.Add(comment.Id);
            db.UpsertEntity<Post>("Posts", postId, post);
        }
        static public List<Comment> GetPostComments(ObjectId postId)
        {
            comments.Clear();
            var post = db.GetEntityById<Post>("Posts", postId);
            var commentsIds = post.CommentsId;
            foreach (var commentId in commentsIds)
            {
                Comment comment = db.GetEntityById<Comment>("Comments", commentId);
                comments.Add(comment);
            }

            return comments;
        }

    }
}
