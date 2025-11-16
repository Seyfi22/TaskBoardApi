using AutoMapper;
using TaskBoardApi.DTOs.Task;
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

            // while updating the user
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Role, opt => opt.Condition(src => src.Role != null))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<Role>(src.Role, true)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // while getting task(s)
            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserId));

            // while posting a new task
            CreateMap<CreateTaskDto, TaskItem>();

            // while updating the task
            CreateMap<UpdateTaskDto, TaskItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
