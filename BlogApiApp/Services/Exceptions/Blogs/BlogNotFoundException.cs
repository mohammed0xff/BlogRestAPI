using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Exceptions.Blogs
{
    public class BlogNotFoundException : NotFoundException
    {
        public BlogNotFoundException(int blogId)
            : base($"Blog wiht id {blogId} does not exist.")
        {
        }
    }
}
