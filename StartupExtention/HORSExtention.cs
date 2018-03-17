using NHORS.NStartupInfo;
using Microsoft.Extensions.DependencyInjection;
using NHORS.NHORS;
using System.Net.Http;

namespace NHORS.NStartupExtention
{
    public static class HORSExtention
    {
        public static void AddHORSExtention(this IServiceCollection services,IStartupInfo startupInfo)
        {
            services.AddTransient<IHORS,HORS>();
            services.AddSingleton(startupInfo);        
            services.AddTransient<HttpClient>();
        }
    }
}
