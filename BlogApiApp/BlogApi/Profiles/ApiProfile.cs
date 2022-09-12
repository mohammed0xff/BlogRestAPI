using AutoMapper;
using Models.ApiModels;
using Models.Entities;

namespace Services.Profiles
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {

            CreateMap<AppUser, AppUserResponse>()
                .ForMember(
                 dest => dest.FullName, 
                 opt => opt.MapFrom(src => src.FirstName + " " + src.LastName)
                );

            CreateMap<Post, PostResponse>();
            CreateMap<PostRequest, Post >();

            CreateMap<Comment, CommentResponse>();
            CreateMap<CommentRequest, Comment>();

            CreateMap<Blog, BlogResponse>();
            CreateMap<BlogRequest, Blog>();

        }
    }
}
