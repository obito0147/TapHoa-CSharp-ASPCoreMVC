using AutoMapper;
using OnTapCSharp.Data;
using OnTapCSharp.ViewModel;

namespace OnTapCSharp.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, KhachHang>();
        }
    }
}
