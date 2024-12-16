using AutoMapper;
using DataAccess.Entities;
using BusinessLogic.DTOs.Order;

namespace BusinessLogic.Configurations.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerUsername, opt => opt.MapFrom(src => src.User.Username));
            
              
        }
    }
}
