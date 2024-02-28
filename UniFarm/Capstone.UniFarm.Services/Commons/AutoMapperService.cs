using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;


namespace Capstone.UniFarm.Services.Commons
{
    public class AutoMapperService : Profile
    {
        public AutoMapperService()
        {
            #region Account Mapping
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();
            #endregion

            #region Category Mapping

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<Category, CategoryRequestUpdate>().ReverseMap();

            #endregion

            #region Area Mapping

            CreateMap<AreaRequestCreate, Area>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            CreateMap<AreaRequestUpdate, Area>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<Area, AreaResponse>();

            #endregion

            #region Apartment Mapping

            CreateMap<ApartmentRequestCreate, Apartment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<ApartmentRequestUpdate, Apartment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<Apartment, ApartmentResponse>();

            #endregion
            
            #region Wallet Mapping
            CreateMap<WalletRequest, Wallet>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));
            #endregion

            CreateMap<FarmHub, FarmHubRequest>().ReverseMap();
            CreateMap<FarmHub, FarmHubRequestUpdate>().ReverseMap();
            CreateMap<FarmHub, FarmHubResponse>().ReverseMap();

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