using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MovieApi.Infrastructure.Data;
using MovieApi.Application.Commands;
using MovieApi.Application.Interfaces;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateMovieCommand).Assembly);
        });
        
        var connectionString = context.Configuration["DatabaseConnectionString"] 
            ?? "Server=JADA-S-PC\\SQLEXPRESS;Database=MovieDb;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;Connection Timeout=30";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddHttpClient();
        
        services.AddLogging();
    })
    .Build();

host.Run();