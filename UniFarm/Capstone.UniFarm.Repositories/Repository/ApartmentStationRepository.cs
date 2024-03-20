using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;

namespace Capstone.UniFarm.Repositories.Repository;

public class ApartmentStationRepository : GenericRepository<ApartmentStation>, IApartmentStationRepository
{
    public ApartmentStationRepository(FTAScript_V1Context context) : base(context)
    {
    }
}