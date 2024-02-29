using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;

namespace Capstone.UniFarm.Repositories.Repository;

public class AreaRepository : GenericRepository<Area>, IAreaRepository
{
    public AreaRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public override void SoftRemove(Area entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
    }

    public new Area Remove(Area entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
        return entity;
    }
}