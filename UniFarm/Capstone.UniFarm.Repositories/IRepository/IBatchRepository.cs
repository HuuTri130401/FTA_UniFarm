using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Repositories.IRepository
{
    public interface IBatchRepository : IGenericRepository<Batch>
    {
        Task<List<Batch>> GetAllBatchesByFarmHubId(Guid farmhubId);
        Task<List<Batch>> GetAllBatchesByFarmHubIdAndBusinessDayId(Guid farmhubId, Guid businessDayId);
        Task<List<Batch>> GetAllBatchesInBusinessDay(Guid collectedHubId, Guid businessDayId);
        Task<List<Batch>> GetAllBatches(Guid collectedHubId);
        Task<Batch> GetAllOrdersInBatch(Guid batchId);
    }
}
