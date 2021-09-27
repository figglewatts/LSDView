using Autofac;
using libLSD.Formats;
using LSDView.Controller;
using LSDView.Controllers;
using LSDView.Controllers.GUI;
using LSDView.Controllers.Headless;
using LSDView.Controllers.Interface;
using LSDView.Models;

namespace LSDView
{
    public class ApplicationModule : Module
    {
        public bool Headless { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigController>().AsSelf().As<IConfigController>();
            builder.RegisterType<FileOpenController>().AsSelf().As<IFileOpenController>();
            builder.RegisterType<UpdateCheckerController>().AsSelf().As<IUpdateCheckerController>();

            if (Headless)
            {
                registerHeadlessTypes(builder);
            }
            else
            {
                registerGUITypes(builder);
            }
        }

        protected void registerGUITypes(ContainerBuilder builder)
        {
            builder.RegisterType<AnimationController>().AsSelf().As<IAnimationController>().SingleInstance();
            builder.RegisterType<CameraController>().AsSelf().As<ICameraController>().SingleInstance();
            builder.RegisterType<GUIExportController>().AsSelf().As<IExportController>().SingleInstance();
            builder.RegisterType<GUILBDController>().AsSelf().As<IFileFormatController<LBD, LBDDocument>>()
                   .SingleInstance();
            builder.RegisterType<GUIMOMController>().AsSelf().As<IFileFormatController<MOM, MOMDocument>>()
                   .SingleInstance();
            builder.RegisterType<GUITIMController>().AsSelf().As<IFileFormatController<TIM, TIMDocument>>()
                   .SingleInstance();
            builder.RegisterType<GUITIXController>().AsSelf().As<IFileFormatController<TIX, TIXDocument>>()
                   .SingleInstance();
            builder.RegisterType<GUITMDController>().AsSelf().As<IFileFormatController<TMD, TMDDocument>>()
                   .SingleInstance();
            builder.RegisterType<TreeController>().AsSelf().As<ITreeController>().SingleInstance();
            builder.RegisterType<VRAMController>().AsSelf().As<IVRAMController>().SingleInstance();
        }

        protected void registerHeadlessTypes(ContainerBuilder builder)
        {
            builder.RegisterType<HeadlessConfigController>().AsSelf().As<IConfigController>().SingleInstance();
            builder.RegisterType<HeadlessLBDController>().AsSelf().As<IFileFormatController<LBD, LBDDocument>>()
                   .SingleInstance();
            builder.RegisterType<HeadlessMOMController>().AsSelf().As<IFileFormatController<MOM, MOMDocument>>()
                   .SingleInstance();
            builder.RegisterType<HeadlessTIMController>().AsSelf().As<IFileFormatController<TIM, TIMDocument>>()
                   .SingleInstance();
            builder.RegisterType<HeadlessTIXController>().AsSelf().As<IFileFormatController<TIX, TIXDocument>>()
                   .SingleInstance();
            builder.RegisterType<HeadlessTMDController>().AsSelf().As<IFileFormatController<TMD, TMDDocument>>()
                   .SingleInstance();
            builder.RegisterType<HeadlessVRAMController>().AsSelf().As<IVRAMController>().SingleInstance();
            builder.RegisterType<HeadlessExportController>().AsSelf().As<IExportController>().SingleInstance();
        }
    }
}
