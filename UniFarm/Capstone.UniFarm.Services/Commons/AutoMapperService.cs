using AutoMapper;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.ViewModels.Account.Response;
using Capstone.UniFarm.Services.ViewModels.Authen.Request;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.Commons
{
    public class AutoMapperService : Profile
    {
        public AutoMapperService()
        {
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<RegisterVM, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();

            CreateMap<FarmHub, FarmHubRequest>().ReverseMap();
            CreateMap<FarmHub, FarmHubResponse>().ReverseMap();

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
        }
    }
}


// Example
/*CreateMap<TransactionHistoryDTO, TransactionHistory>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => _unitOfWork.CustomerRepo.GetById(src.CustomerId)))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => _unitOfWork.PaymentRepo.GetById(src.PaymentId)))
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => _unitOfWork.AccountTypeRepo.GetById(src.AccountTypeId)))
                .AfterMap(async (src, dest, context) =>
                {
                    var accountType = await _unitOfWork.AccountTypeRepo.GetByIdAsync(src.AccountTypeId);
                    dest.AccountType = accountType;

                    var payment = await _unitOfWork.PaymentRepo.GetByIdAsync(src.PaymentId);
                    dest.Payment = payment;
                })
                 .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.PaymentDate))
                 .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.PaymentDate.AddDays(30)))
                 .ReverseMap();*/
