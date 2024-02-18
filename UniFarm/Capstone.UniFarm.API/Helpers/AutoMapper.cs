using AutoMapper;
using Capstone.UniFarm.API.ViewModels.Category.Request;
using Capstone.UniFarm.API.ViewModels.Category.Response;
using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.API.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<Category, CategoryRequestUpdate>().ReverseMap();
        }
    }
}
