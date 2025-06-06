using AutoMapper;
using QLCHNT.Const;
using QLCHNT.Controllers;
using QLCHNT.Dto.Cart;
using QLCHNT.Dto.Category;
using QLCHNT.Dto.Order;
using QLCHNT.Dto.Product;
using QLCHNT.Dto.User;
using QLCHNT.Entity;

namespace QLCHNT.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapper tử UserRegisterRequet sang userEntity

            CreateMap<UserRegister, UserEntity>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Role, o => o.MapFrom(src => Enums.Role.User))
                // => Không mapper mật khẩu => Ignore
                .ForMember(dest => dest.Password, o => o.Ignore());
            CreateMap<UserUpdate, UserEntity>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // nếu không cho cập nhật Role
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // nếu không cập nhật Password
            CreateMap<UserEntity, UserDto>();
            CreateMap<UserCreateRequest, UserEntity>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now));



            CreateMap<CategoryCreateRequest, CategoryEntity>();
            CreateMap<CategoryEntity, CategoryDto>();
            CreateMap<CategoryUpdateRequest, CategoryEntity>();

            CreateMap<ProductEntity, ProductDto>()
                .ForMember(dest => dest.CategoryName, o => o.MapFrom(src => src.Category.Name));
             
            CreateMap<ProductCreateRequest, ProductEntity>();
            CreateMap<ProductUpdate, ProductEntity>();

            
                

            
            CreateMap<CartEntity, CartDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<AddCartItemDto, CartItemEntity>();
            CreateMap<CartItemEntity, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.ToString()));


            CreateMap<OrderEntity, OrderDto>()
                .ForMember(dest => dest.FullName, otp => otp.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Address, otp => otp.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.Phone, otp => otp.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.TotalPrice, o => o.MapFrom(src => src.OrderDetails.Sum(od => od.Price*od.Quantity)))
                .ForMember(dest => dest.OrderDetails , opt => opt.MapFrom(src => src.OrderDetails));
            
            CreateMap<OrderDetailEntity, OrderDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}
