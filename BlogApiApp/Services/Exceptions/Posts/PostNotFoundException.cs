using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions.Posts
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(int postId)
            : base($"Post wiht id {postId} does not exist.")
        {

        }
    }
}
