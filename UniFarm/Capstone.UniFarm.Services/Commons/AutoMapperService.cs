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

            CreateMap<Account, AccountAndFarmHubRequest>().ReverseMap();

            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<AccountRequestCreate, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();
            CreateMap<FarmHubRegisterRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();
            CreateMap<AccountRequestUpdate, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => System.DateTime.Now))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role))
                .ReverseMap();

            CreateMap<Account, AboutMeResponse.AboutCustomerResponse>();
            CreateMap<Account, AboutMeResponse.AboutFarmHubResponse>();
            CreateMap<Account, AboutMeResponse.AboutCollectedStaffResponse>();
            CreateMap<Account, AboutMeResponse.AboutStationStaffResponse>();
            CreateMap<Account, AboutMeResponse.AboutAdminResponse>();
            CreateMap<Account, AboutMeResponse.StaffResponse>();
            CreateMap<Account, AboutMeResponse.CustomerResponseSimple>();
            #endregion

            #region Role Mapping

            CreateMap<AccountRoleRequest, AccountRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));

            CreateMap<AccountRoleRequestUpdate, AccountRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                        String.IsNullOrEmpty(src.Status) ? EnumConstants.ActiveInactiveEnum.ACTIVE : src.Status));

            CreateMap<Station, StationResponse>()
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area));

            CreateMap<Station, StationResponse.StationResponseSimple>();
            #endregion

            #region CollectedHub Mapping
            CreateMap<CollectedHubRequestCreate, CollectedHub>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.ActiveInactiveEnum.ACTIVE));


            CreateMap<CollectedHubRequestUpdate, CollectedHub>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    String.IsNullOrEmpty(src.Status) ? EnumConstants.ActiveInactiveEnum.ACTIVE : src.Status));

            CreateMap<CollectedHub, CollectedHubResponse>();
            CreateMap<CollectedHub, CollectedHubResponseContainStaffs>();
            #endregion

            #region Payment Mapping
            CreateMap<PaymentRequestCreate, Payment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PaymentDay, opt => opt.MapFrom(src => DateTime.Now));
            #endregion

            
            CreateMap<ApartmentStation, ApartmentStationResponse>()
                .ForMember(dest => dest.Apartment, opt => opt.MapFrom(src => src.Apartment))
                .ForMember(dest => dest.Station, opt => opt.MapFrom(src => src.Station));

            CreateMap<ProductItem, ProductItemResponseForCustomer>();
            CreateMap<OrderDetail, OrderDetailResponseForCustomer>();
            CreateMap<OrderDetail, OrderDetailResponseForFarmHub>()
                .ForMember(dest => dest.ProductItemTitle, opt => opt.MapFrom(src => src.ProductItem != null ? src.ProductItem.Title : null))
                .ReverseMap();


            CreateMap<TransferRequestCreate, Transfer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EnumConstants.StationUpdateTransfer.Pending));

            CreateMap<Transfer, TransferResponse>()
                .ForMember(dest => dest.Collected, opt => opt.MapFrom(src => src.Collected))
                .ForMember(dest => dest.Station, opt => opt.MapFrom(src => src.Station))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

            CreateMap<FarmHub, FarmHubRequest>().ReverseMap();
            CreateMap<FarmHub, FarmHubRequestUpdate>().ReverseMap();
            CreateMap<FarmHub, FarmHubResponse>().ReverseMap();
            CreateMap<FarmHub, AccountAndFarmHubRequest>()
                .ForMember(dest => dest.FarmHubName, act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.FarmHubCode, act => act.MapFrom(src => src.Code))
                .ForMember(dest => dest.FarmHubImage, act => act.MapFrom(src => src.Image))
                .ForMember(dest => dest.FarmHubAddress, act => act.MapFrom(src => src.Address))
                .ReverseMap();

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
            CreateMap<ProductItem, ProductItemResponse>()
                 .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                 .ForMember(dest => dest.CommissionFee, opt => opt.MapFrom(src => src.Product != null && src.Product.Category != null ? src.Product.Category.SystemPrice : 0))
                 .ForMember(dest => dest.Sold, opt => opt.MapFrom(src => src.ProductItemInMenus.Sum(item => item.Sold)))
                 .ReverseMap();
            CreateMap<ProductItem, ProductItemResponseForCustomerView>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ReverseMap();

            CreateMap<ProductItemInMenu, ProductItemInMenuRequest>().ReverseMap();
            CreateMap<ProductItemInMenu, ProductItemInMenuResponse>().ReverseMap();
            CreateMap<ProductItemInMenu, ProductItemInMenuResponseForCustomer>().ReverseMap();

            CreateMap<BusinessDay, BusinessDayRequest>().ReverseMap();
            CreateMap<BusinessDay, BusinessDayResponse>().ReverseMap();

            CreateMap<Batch, BatchRequest>().ReverseMap();
            CreateMap<Batch, BatchRequestUpdate>().ReverseMap();
            CreateMap<Batch, BatchResponse>()
                .ForMember(dest => dest.BusinessDayName, opt => opt.MapFrom(src => src.BusinessDay != null ? src.BusinessDay.Name : null))
                .ForMember(dest => dest.BusinessDayOpen, opt => opt.MapFrom(src => src.BusinessDay.OpenDay))
                .ForMember(dest => dest.CollectedHubName, opt => opt.MapFrom(src => src.Collected != null ? src.Collected.Name : null))
                .ForMember(dest => dest.CollectedHubAddress, opt => opt.MapFrom(src => src.Collected != null ? src.Collected.Address : null))
                .ReverseMap();
            CreateMap<Batch, BatchDetailResponse>()
                .ForMember(dest => dest.FarmHubName, opt => opt.MapFrom(src => src.FarmHub != null ? src.FarmHub.Name : null))
                .ForMember(dest => dest.CollectedHubName, opt => opt.MapFrom(src => src.Collected != null ? src.Collected.Name : null))
                .ForMember(dest => dest.BusinessDayName, opt => opt.MapFrom(src => src.BusinessDay != null ? src.BusinessDay.Name : null))
                .ForMember(dest => dest.BusinessDayOpen, opt => opt.MapFrom(src => src.BusinessDay.OpenDay))
                .ReverseMap();

            CreateMap<Order, OrderResponseToProcess>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.UserName : null))
                .ForMember(dest => dest.BusinessDayName, opt => opt.MapFrom(src => src.BusinessDay != null ? src.BusinessDay.Name : null))
                .ForMember(dest => dest.BusinessDayOpen, opt => opt.MapFrom(src => src.BusinessDay.OpenDay))
                .ReverseMap();
            CreateMap<Order, OrdersInBatchResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.UserName : null))
                //.ForMember(dest => dest.BusinessDayName, opt => opt.MapFrom(src => src.BusinessDay != null ? src.BusinessDay.Name : null))
                //.ForMember(dest => dest.BusinessDayOpen, opt => opt.MapFrom(src => src.BusinessDay.OpenDay))
                .ReverseMap();

            CreateMap<FarmHubSettlement, FarmHubSettlementRequest>().ReverseMap();
            CreateMap<FarmHubSettlement, FarmHubSettlementResponse>()
                .ForMember(dest => dest.BusinessDayName, opt => opt.MapFrom(src => src.BusinessDay != null ? src.BusinessDay.Name : null))
                .ForMember(dest => dest.BusinessOpenday, opt => opt.MapFrom(src => src.BusinessDay.OpenDay))
                .ReverseMap();

            CreateMap<Transaction, TransactionResponse>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.HasValue ? src.Amount.Value : 0))
                .ForMember(dest => dest.PayerName, opt => opt.MapFrom(src => src.PayerWallet.Account.UserName))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order));
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