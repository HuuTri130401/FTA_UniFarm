using AutoMapper;
using Capstone.UniFarm.API.ViewModels.ModelRequests;
using Capstone.UniFarm.API.ViewModels.ModelResponses;
using Capstone.UniFarm.Domain.Models;

namespace Capstone.UniFarm.API.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Category, CategoryRequest>();
            CreateMap<Category, CategoryResponse>();
        }
    }
}
