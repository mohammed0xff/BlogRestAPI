using AutoMapper;
using Models.ApiModels;
using Models.ApiModels.ResponseDTO;
using Models.Entities;

namespace Services.Profiles
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<BlogRequest, Blog>();
            CreateMap<Blog, BlogResponse>();

            CreateMap<PostRequest, Post>();
            CreateMap<Post, PostResponse>()
               .ForMember(
                    dest => dest.Tags,
                    opt => opt.MapFrom(
                        source => source.PostTags
                        )
                );

            CreateMap<Tag, TagResponse>();
            CreateMap<PostTag, TagResponse>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(
                        source => source.Tag.Name
                        )
                    );

            CreateMap<CommentRequest, Comment>();
            CreateMap<Comment, CommentResponse>();

            CreateMap<AppUser, AppUserResponse>()
                .ForMember(
                     dest => dest.FullName,
                     opt => opt.MapFrom(
                         source => source.FirstName + " " + source.LastName
                         )
                );
            CreateMap<AppUser, AppUserAdminResponse>()
                .IncludeBase<AppUser, AppUserResponse>();

        }
    }
}
