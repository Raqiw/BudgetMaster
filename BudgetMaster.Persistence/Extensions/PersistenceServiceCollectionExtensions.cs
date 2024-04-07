using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetMaster.Persistence.Extensions
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static void AddBudgetDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BudgetMasterDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(nameof(BudgetMasterDbContext)));
            });
        }
    }
}
