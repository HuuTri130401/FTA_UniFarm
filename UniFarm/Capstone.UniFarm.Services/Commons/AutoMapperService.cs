using AutoMapper;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.UnitOfWork;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;


namespace Capstone.UniFarm.Services.Commons
{
    public class AutoMapperService : Profile
    {

        public AutoMapperService(
        )
        {
            #region Account Mapping

            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<AccountRequestCreate, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();

            CreateMap<AccountRequestUpdate, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.Now))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Status) ? EnumConstants.ActiveInactiveEnum.ACTIVE : src.Status))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role))
                .ReverseMap();
                
            CreateMap<Account, AboutMeResponse.AboutCustomerResponse>();
            CreateMap<Account, AboutMeResponse.AboutFarmHubResponse>();
            CreateMap<Account, AboutMeResponse.AboutCollectedStaffResponse>();
            CreateMap<Account, AboutMeResponse.AboutStationStaffResponse>();
            CreateMap<Account, AboutMeResponse.AboutAdminResponse>();
            CreateMap<Account, AboutMeResponse.StaffResponse>();
            #endregion
            
            
            #region Role Mapping

            CreateMap<AccountRoleRequest, AccountRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));

            CreateMap<AccountRoleRequestUpdate, AccountRole>();
            #endregion

            #region Category Mapping

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<Category, CategoryResponseForCustomer>().ReverseMap();
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

            #region Station Mapping

            CreateMap<StationRequestCreate, Station>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));

            CreateMap<StationRequestUpdate, Station>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status,opt => opt.MapFrom(src =>
                        String.IsNullOrEmpty(src.Status) ? EnumConstants.ActiveInactiveEnum.ACTIVE : src.Status));

            CreateMap<Station, StationResponse>();

            #endregion

            #region CollectedHub Mapping
            CreateMap<CollectedHubRequestCreate, CollectedHub>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));
            
            
            CreateMap<CollectedHubRequestUpdate, CollectedHub>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status,opt => opt.MapFrom(src =>
                    String.IsNullOrEmpty(src.Status) ? EnumConstants.ActiveInactiveEnum.ACTIVE : src.Status));
            
            CreateMap<CollectedHub, CollectedHubResponse>();
            CreateMap<CollectedHub, CollectedHubResponseContainStaffs>();
            #endregion

            CreateMap<FarmHub, FarmHubRequest>().ReverseMap();
            CreateMap<FarmHub, FarmHubRequestUpdate>().ReverseMap();
            CreateMap<FarmHub, FarmHubResponse>().ReverseMap();

            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductRequestUpdate>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();

            CreateMap<ProductImage, ProductImageRequest>().ReverseMap();
            CreateMap<ProductImage, ProductImageRequestUpdate>().ReverseMap();
            CreateMap<ProductImage, ProductImageResponse>().ReverseMap();

            CreateMap<Menu, MenuRequest>().ReverseMap();
            CreateMap<Menu, MenuRequestUpdate>().ReverseMap();
            CreateMap<Menu, MenuResponse>().ReverseMap();

            CreateMap<ProductItem, ProductItemRequest>().ReverseMap();
            CreateMap<ProductItem, ProductItemRequestUpdate>().ReverseMap();
            CreateMap<ProductItem, ProductItemResponse>().ReverseMap();

            CreateMap<ProductItemInMenu, ProductItemInMenuRequest>().ReverseMap();
            CreateMap<ProductItemInMenu, ProductItemInMenuResponse>().ReverseMap();

            CreateMap<BusinessDay, BusinessDayRequest>().ReverseMap();
            CreateMap<BusinessDay, BusinessDayResponse>().ReverseMap();
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