using BusinessLogic.DTOs.Product;
using AutoMapper;
using BusinessLogic.DTOs.Auth;
using BusinessLogic.DTOs.Lab;
using BusinessLogic.DTOs.User;
using BusinessLogic.DTOs.Subcategory;
using BusinessLogic.DTOs.Order;
using DataAccess.Entities;

namespace BusinessLogic.Configurations.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, ReadUserDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ? "Active" : "Banned"))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName).ToList()));   
            //Product mappings
               CreateMap<Product, ReadProductDto>()
            .ForMember(dest => dest.LabName,
                      opt => opt.MapFrom(src => src.Lab != null ? src.Lab.LabName : null))
            .ForMember(dest => dest.SubcategoryName,
                      opt => opt.MapFrom(src => src.Subcategory != null ? src.Subcategory.SubcategoryName : null));

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // MappingsAuthentication
            CreateMap<UserRegistrationDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true)); // Default status            

            // Lab Mappings
            CreateMap<Lab, ReadLabDto>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

            CreateMap<CreateLabDto, Lab>();
            CreateMap<UpdateLabDto, Lab>();
            CreateMap<Lab, ReadLabSimpleDto>();

            // Subcategory mappings
            CreateMap<Subcategory, ReadSubcategoryDto>();
            // Order -> OrderDto
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerUsername,
                    opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.OrderDate,
                    opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.DeliveryStatus,
                    opt => opt.MapFrom(src => src.DeliveryStatus))
                .ForMember(dest => dest.SupportStatus,
                    opt => opt.MapFrom(src => src.SupportStatus))
                .ForMember(dest => dest.OrderDetails,
                    opt => opt.MapFrom(src => src.OrderDetails));

            // OrderDetail -> OrderDetailDto
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductDescription,
                    opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price ?? 0m));
           
            // CreateSubcategoryDto->Subcategory
            CreateMap<Subcategory, ReadSubcategoryDto>();

            CreateMap<CreateSubcategoryDto, Subcategory>()
                .ForMember(dest => dest.Products, opt => opt.Ignore());
        }
    }
}
