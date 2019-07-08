using System;
using System.Data.Entity;
using System.Web;
using Auth0.AuthenticationApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using Portalia.Auth0.Constants;
using Portalia.Auth0.TokenGenerator;
using Portalia.Core.Dtos.Message;
using Portalia.Core.Entity;
using Portalia.Core.Interface.Service;
using Portalia.Models;
using Portalia.Repository;
using Portalia.Service;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;

namespace Portalia.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container
                .RegisterType<IDataContextAsync, PortaliaContext>(new PerRequestLifetimeManager())
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerRequestLifetimeManager())
                .RegisterType<PortaliaContext>(new PerRequestLifetimeManager())
                .RegisterType<AmarisContext>(new PerRequestLifetimeManager())
                .RegisterType<WebsiteApplicationContext>(new PerRequestLifetimeManager())
                .RegisterType(typeof(IRepositoryAsync<>), typeof(Repository<>), new PerRequestLifetimeManager())
                .RegisterType<IProposalService, ProposalService>(new PerRequestLifetimeManager())
                .RegisterType<IDocumentService, DocumentService>(new PerRequestLifetimeManager())
                .RegisterType<IRefDataService, RefDataService>(new PerRequestLifetimeManager())
                .RegisterType<IWorkContractService, WorkContractService>(new PerRequestLifetimeManager())
                .RegisterType<IAttributeTypeService, AttributeTypeService>(new PerRequestLifetimeManager())
                .RegisterType<IUserProfileAttributeService, UserProfileAttributeService>(new PerRequestLifetimeManager())
                .RegisterType<IDataSourceService, DataSourceService>(new PerRequestLifetimeManager())
                .RegisterType<IApplicationForm, ApplicationFormService>(new PerRequestLifetimeManager())
                .RegisterType<IUserProfileService, UserProfileService>(new PerRequestLifetimeManager())
                .RegisterType<IMigrationService, MigrationService>(new PerRequestLifetimeManager())
                .RegisterType<ISecuredTable, SecuredTable>(new PerRequestLifetimeManager())
                .RegisterType<ICreateUserProfileBasedOnExternalLogin, LinkedinUserProfile>("LinkedIn", new PerRequestLifetimeManager());

            container.RegisterType<DbContext, ApplicationDbContext>(new PerRequestLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(new PerRequestLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new PerRequestLifetimeManager());
            //container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<AuthenticationApiClient>(new InjectionConstructor(new Uri(Auth0Constants.BaseUrl)));
            container.RegisterType<ITokenGenerator, Auth0TokenGenerator>();
        }
    }
}
