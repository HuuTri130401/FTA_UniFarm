using Capstone.UniFarm.Domain.Data;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Repositories.IRepository;

namespace Capstone.UniFarm.Repositories.Repository;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(FTAScript_V1Context context) : base(context)
    {
    }
    
    public override void SoftRemove(Payment entity)
    {
        return;
    }

    public override void Update(Payment entity)
    {
        return;
    }
    
}