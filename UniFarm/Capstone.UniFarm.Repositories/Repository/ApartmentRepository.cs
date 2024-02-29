using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;

namespace Capstone.UniFarm.Repositories.Repository;

public class ApartmentRepository : GenericRepository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public override void SoftRemove(Apartment entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
    }

    public new Apartment Remove(Apartment entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
        return entity;
    }

}