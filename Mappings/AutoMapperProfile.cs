using AutoMapper;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Ánh xạ User
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Course
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.CourseModules, opt => opt.MapFrom(src => src.CourseModules));
            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.CourseModules, opt => opt.MapFrom(src => src.CourseModules));
            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.CourseModules, opt => opt.MapFrom(src => src.CourseModules))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Blog
            CreateMap<Blog, BlogDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
            CreateMap<CreateBlogDto, Blog>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
            CreateMap<UpdateBlogDto, Blog>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Cart
            CreateMap<Cart, CartDto>();
            CreateMap<CreateCartDto, Cart>();
            CreateMap<UpdateCartDto, Cart>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Contact
            CreateMap<Contact, ContactDto>();
            CreateMap<CreateContactDto, Contact>();

            // Ánh xạ Lesson
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();
            CreateMap<UpdateLessonDto, Lesson>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Module
            CreateMap<Module, ModuleDto>()
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons));
            CreateMap<CreateModuleDto, Module>()
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons));
            CreateMap<UpdateModuleDto, Module>()
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Order
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Progress
            CreateMap<Progress, ProgressDto>()
                .ForMember(dest => dest.CompletedLessons, opt => opt.MapFrom(src => src.CompletedLessons));
            CreateMap<InitProgressDto, Progress>();

            // Ánh xạ Review
            CreateMap<Review, ReviewDto>();
            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Voucher
            CreateMap<Voucher, VoucherDto>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.CourseIds));
            CreateMap<CreateVoucherDto, Voucher>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.CourseIds));
            CreateMap<UpdateVoucherDto, Voucher>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.CourseIds))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Ánh xạ Wishlist
            CreateMap<Wishlist, WishlistDto>();
            CreateMap<CreateWishlistDto, Wishlist>();
        }
    }
}
