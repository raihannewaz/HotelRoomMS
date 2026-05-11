using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.FileUpload;
using Common.Core.FileUpload;
using Common.Core.Jwt;
using HotelRoomMS.Application.Hotels;
using Microsoft.Extensions.DependencyInjection;

namespace HotelRoomMS.Application
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMyMediaTR(typeof(ServiceConfiguration).Assembly);
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddMyMediaTR();
            services.AddScoped<ISecurityContextAccessor, SecurityContextAccessor>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
