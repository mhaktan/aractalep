using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using aractalep.EntityFrameworkCore;

namespace aractalep.Web.Host
{
    [DependsOn(typeof(aractalepApplicationModule), typeof(aractalepEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class aractalepWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(aractalepApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(aractalepWebHostModule).GetAssembly());
        }
    }
}
