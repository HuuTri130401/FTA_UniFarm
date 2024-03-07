using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;

namespace Capstone.UniFarm.Repositories.Repository;

public class CollectedHubRepository : GenericRepository<CollectedHub>, ICollectedHubRepository
{
    public CollectedHubRepository(FTAScript_V1Context context) : base(context)
    {
    }

    public override void SoftRemove(CollectedHub entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
    }

    public new CollectedHub Remove(CollectedHub entity)
    {
        entity.Status = EnumConstants.ActiveInactiveEnum.INACTIVE;
        _dbSet.Update(entity);
        return entity;
    }
}