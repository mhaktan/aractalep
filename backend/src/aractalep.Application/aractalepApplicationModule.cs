using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace aractalep
{
    [DependsOn(typeof(aractalepCoreModule), typeof(AbpAutoMapperModule))]
    public class aractalepApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(aractalepApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(aractalepApplicationModule).GetAssembly());
        }
    }
}
