using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using ManageMyNotificationsBusinessLayer.Proxy;
using ManageMyNotificationsBusinessLayer.Services;
using ManageMyNotificationsBusinessLayer.Services.XMatters;
using ManageMyNotificationsMVC.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ManageMyNotificationsMVC.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(ManageMyNotificationsMVC.App_Start.NinjectWebCommon), "Stop")]

namespace ManageMyNotificationsMVC.App_Start
{
    [ExcludeFromCodeCoverage]
    public class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        //private static IKernel CreateKernel()
        //{
        //    var kernel = new StandardKernel();
        //    kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
        //    kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
        //    RegisterServices(kernel);

        //    return kernel;
        //}


        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                // This binds all 'IFoo' interfaces to their respective 'Foo' classes.
                kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }


        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Rebind<IElmahWrapper>().To<ElmahWrapper>().Named("ElmahWrapper");

            kernel.Rebind<ISalesforceNotificationServiceProxy>().To<SalesforceNotificationServiceProxy>();
            kernel.Rebind<IXMattersIntegrationServiceProxy>().To<XMattersIntegrationServiceProxy>();

            kernel.Rebind<ISalesforceNotificationService>().To<SalesforceNotificationServiceClient>();
            kernel.Rebind<IXMattersIntegrationService>().To<XMattersIntegrationServiceClient>();
            
            kernel.Rebind<IXMattersServiceHelper>().To<XMattersServiceHelper>();
            kernel.Rebind<IXMatterPersonService>().To<XMatterPersonService>();
            kernel.Rebind<IXMatterGroupService>().To<XMatterGroupService>();

            kernel.Rebind<HttpClient>().To<HttpClient>();

            kernel.Rebind<IPersonBusinessLogic>().To<PersonBusinessLogic>();
            kernel.Rebind<IAuditLogService>().To<AuditLogService>();
            kernel.Rebind<HttpClient>().To<HttpClient>();
            kernel.Rebind<ICustomerNotificationsAPIHelper>().To<CustomerNotificationsAPIHelper>();
            kernel.Rebind<ICustomerNotificationsAPIService>().To<CustomerNotificationsAPIService>();

        }
    }
}