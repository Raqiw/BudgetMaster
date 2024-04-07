using BudgetMaster.Application.Services;
using BudgetMaster.IApplication.Services;
using BudgetMaster.IPersistence.Repositories;
using BudgetMaster.Persistence.Repositories;
using BudgetMaster.Persistence.Extensions;
using Microsoft.OpenApi.Models;

namespace BudgetMaster.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "BudgetMaster.API - V1",
                        Version = "v1"
                    }
                    );
                    var filePath = Path.Combine(System.AppContext.BaseDirectory, "BudgetMaster.API.xml");
                    c.IncludeXmlComments(filePath);
                });

                var configuration = builder.Configuration;
                builder.Services.AddBudgetDbContext(configuration);

                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IBudgetCalculationRepository, BudgetCalculationRepository>();
                //builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
                //builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IBudgetCalculationService, BudgetCalculationService>();
            }


            var app = builder.Build();
            {
                app.UseExceptionHandler("/error");
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }


        }
    }
}
