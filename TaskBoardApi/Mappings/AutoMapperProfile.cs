using AutoMapper;
using TaskBoardApi.DTOs.User;
using TaskBoardApi.Model.Entities;
using TaskBoardApi.Model.Enums;

namespace TaskBoardApi.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // while getting user(s)
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<TaskItem, TaskInUserDto>();

            // while posting a new user
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<Role>(src.Role, true)));

            CreateMap<CreateTaskInCreateUserDto, TaskItem>();
        }
    }
}
