using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using Portalia.Repository.Mapping.WebApplication;
using System.Data.SqlClient;
using Portalia.Core.Entity.WebApplication;

namespace Portalia.Repository
{
    public class WebsiteApplicationContext : DbContext
    {
        public WebsiteApplicationContext()
            : base("name=WebsiteApplicationEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.AddFromAssembly(typeof(WebsiteApplicationContext).Assembly);
            modelBuilder.Configurations.Add(new ApplicationDocumentMapping());
            modelBuilder.Configurations.Add(new ApplicationFormMapping());
            modelBuilder.Configurations.Add(new ApplicationFormDetailMapping());
            modelBuilder.Configurations.Add(new ApplicationFormStatusMapping());
            modelBuilder.Configurations.Add(new ApplicationSourceMapping());
            modelBuilder.Configurations.Add(new DocumentTypeMapping());
            modelBuilder.Configurations.Add(new GenderMapping());
            modelBuilder.Configurations.Add(new TitleMapping());
            modelBuilder.Configurations.Add(new Website_ContactRequestMapping());
        }

        public virtual DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public virtual DbSet<ApplicationForm> ApplicationForms { get; set; }
        public virtual DbSet<ApplicationFormDetail> ApplicationFormDetails { get; set; }
        public virtual DbSet<ApplicationFormStatus> ApplicationFormStatus { get; set; }
        public virtual DbSet<ApplicationSource> ApplicationSources { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<Website_ContactRequest> Website_ContactRequest { get; set; }

        public virtual int AddApplicationFormDocument(Nullable<int> applicationFormId, string filename, string extension, byte[] binary, Nullable<byte> typeId)
        {
            var applicationFormIdParameter = applicationFormId.HasValue ?
                new SqlParameter("@applicationFormId", applicationFormId) :
                new SqlParameter("@applicationFormId", DBNull.Value);

            var filenameParameter = filename != null ?
                new SqlParameter("@filename", filename) :
                new SqlParameter("@filename", DBNull.Value);

            var extensionParameter = extension != null ?
                new SqlParameter("@extension", extension) :
                new SqlParameter("@extension", DBNull.Value);

            var binaryParameter = binary != null ?
                new SqlParameter("@binary", binary) :
                new SqlParameter("@binary", DBNull.Value);

            var typeIdParameter = typeId.HasValue ?
                new SqlParameter("@typeId", typeId) :
                new SqlParameter("@typeId", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("exec AddApplicationFormDocument @applicationFormId,@filename,@extension,@binary,@typeId", applicationFormIdParameter, filenameParameter, extensionParameter, binaryParameter, typeIdParameter);
        }

        public virtual ObjectResult<Nullable<int>> CreateApplicationForm(string email, Nullable<byte> applicationSourceId, Nullable<int> jobOfferId, string firstName, string lastname, Nullable<byte> titleId, Nullable<byte> genderId, Nullable<System.DateTime> birthDate, string preferredLanguageId, string phoneNumber, string residenceCountryId, Nullable<int> residenceCityId, string residenceZipCode, string residenceAddress, string citizenshipCountryId, string utmSource, string utmMedium, string utmCampaign)
        {
            var emailParameter = email != null ?
                new SqlParameter("@email", email) :
                new SqlParameter("@email", string.Empty);

            var applicationSourceIdParameter = applicationSourceId.HasValue ?
                new SqlParameter("@applicationSourceId", applicationSourceId) :
                new SqlParameter("@applicationSourceId", DBNull.Value);

            var jobOfferIdParameter = jobOfferId.HasValue ?
                new SqlParameter("@jobOfferId", jobOfferId) :
                new SqlParameter("@jobOfferId", DBNull.Value);

            var firstNameParameter = firstName != null ?
                new SqlParameter("@firstName", firstName) :
                new SqlParameter("@firstName", DBNull.Value);

            var lastnameParameter = lastname != null ?
                new SqlParameter("@lastname", lastname) :
                new SqlParameter("@lastname", DBNull.Value);

            var titleIdParameter = titleId.HasValue ?
                new SqlParameter("@titleId", titleId) :
                new SqlParameter("@titleId", DBNull.Value);

            var genderIdParameter = genderId.HasValue ?
                new SqlParameter("@genderId", genderId) :
                new SqlParameter("@genderId", DBNull.Value);

            var birthDateParameter = birthDate.HasValue ?
                new SqlParameter("@birthDate", birthDate) :
                new SqlParameter("@birthDate", DBNull.Value);

            var preferredLanguageIdParameter = preferredLanguageId != null ?
                new SqlParameter("@preferredLanguageId", preferredLanguageId) :
                new SqlParameter("@preferredLanguageId", DBNull.Value);

            var phoneNumberParameter = phoneNumber != null ?
                new SqlParameter("@phoneNumber", phoneNumber) :
                new SqlParameter("@phoneNumber", DBNull.Value);

            var residenceCountryIdParameter = residenceCountryId != null ?
                new SqlParameter("@residenceCountryId", residenceCountryId) :
                new SqlParameter("@residenceCountryId", DBNull.Value);

            var residenceCityIdParameter = residenceCityId.HasValue ?
                new SqlParameter("@residenceCityId", residenceCityId) :
                new SqlParameter("@residenceCityId", DBNull.Value);

            var residenceZipCodeParameter = residenceZipCode != null ?
                new SqlParameter("@residenceZipCode", residenceZipCode) :
                new SqlParameter("@residenceZipCode", DBNull.Value);

            var residenceAddressParameter = residenceAddress != null ?
                new SqlParameter("@residenceAddress", residenceAddress) :
                new SqlParameter("@residenceAddress", DBNull.Value);

            var citizenshipCountryIdParameter = citizenshipCountryId != null ?
                new SqlParameter("@citizenshipCountryId", citizenshipCountryId) :
                new SqlParameter("@citizenshipCountryId", DBNull.Value);

            var utmSourceParameter = utmSource != null ?
                new SqlParameter("@UtmSource", utmSource) :
                new SqlParameter("@UtmSource", DBNull.Value);

            var utmMediumParameter = utmMedium != null ?
                new SqlParameter("@UtmMedium", utmMedium) :
                new SqlParameter("@UtmMedium", DBNull.Value);

            var utmCampaignParameter = utmCampaign != null ?
                new SqlParameter("@UtmCampaign", utmCampaign) :
                new SqlParameter("@UtmCampaign", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<Nullable<int>>("exec CreateApplicationForm @email,@applicationSourceId,@jobOfferId,@firstName,@lastname,@titleId,@genderId,@birthDate,@preferredLanguageId,@phoneNumber,@residenceCountryId,@residenceCityId,@residenceZipCode,@residenceAddress,@citizenshipCountryId,@UtmSource,@UtmMedium,@UtmCampaign", emailParameter, applicationSourceIdParameter, jobOfferIdParameter, firstNameParameter, lastnameParameter, titleIdParameter, genderIdParameter, birthDateParameter, preferredLanguageIdParameter, phoneNumberParameter, residenceCountryIdParameter, residenceCityIdParameter, residenceZipCodeParameter, residenceAddressParameter, citizenshipCountryIdParameter, utmSourceParameter, utmMediumParameter, utmCampaignParameter);
        }

        public virtual int DeleteWebAppForm(Nullable<int> applicationFormId)
        {
            var applicationFormIdParameter = applicationFormId.HasValue ?
                new ObjectParameter("applicationFormId", applicationFormId) :
                new ObjectParameter("applicationFormId", typeof(int));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteWebAppForm", applicationFormIdParameter);
        }

        public virtual ObjectResult<GetAppDocument_Result> GetAppDocument(Nullable<int> appFormId)
        {
            var appFormIdParameter = appFormId.HasValue ?
                new ObjectParameter("AppFormId", appFormId) :
                new ObjectParameter("AppFormId", typeof(int));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetAppDocument_Result>("GetAppDocument", appFormIdParameter);
        }

        public virtual ObjectResult<GetWebAppForm_Result> GetWebAppForm()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetWebAppForm_Result>("GetWebAppForm");
        }

        public virtual int UpdateApplicationFormStatus(Nullable<int> applicationFormId, Nullable<int> applicationFormStatus)
        {
            var applicationFormIdParameter = applicationFormId.HasValue ?
                new SqlParameter("@applicationFormId", applicationFormId) :
                new SqlParameter("@applicationFormId", DBNull.Value);

            var applicationFormStatusParameter = applicationFormStatus.HasValue ?
                new SqlParameter("@applicationFormStatus", applicationFormStatus) :
                new SqlParameter("@applicationFormStatus", DBNull.Value);

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("exec UpdateApplicationFormStatus @applicationFormId,@applicationFormStatus", applicationFormIdParameter, applicationFormStatusParameter);
        }
    }
}
