using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;

using Aliencube.Forem.DevTo;

using DevRelKr.PublishDevTo.FunctionApp.Helpers;
using DevRelKr.PublishDevTo.FunctionApp.Models;
using DevRelKr.PublishDevTo.FunctionApp.Services;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[assembly: FunctionsStartup(typeof(DevRelKr.PublishDevTo.FunctionApp.StartUp))]
namespace DevRelKr.PublishDevTo.FunctionApp
{
    /// <summary>
    /// This represents the entity for the IoC container.
    /// </summary>
    public class StartUp : FunctionsStartup
    {
        private const string StorageConnectionStringKey = "AzureWebJobsStorage";

        /// <inheritdoc />
        public override void Configure(IFunctionsHostBuilder builder)
        {
            this.ConfigureAppSettings(builder.Services);
            this.ConfigureHttpClients(builder.Services);
            this.ConfigureApiClients(builder.Services);
            this.ConfigureSerialisers(builder.Services);
            this.ConfigureServices(builder.Services);
            this.ConfigureHelpers(builder.Services);
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            var settings = new AppSettings();

            services.AddSingleton(settings);
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        private void ConfigureApiClients(IServiceCollection services)
        {
            services.AddSingleton<IDEVAPIbeta, DEVAPIbeta>(p =>
            {
                var client = p.GetService<HttpClient>();

                return new DEVAPIbeta(client, false);
            });
        }

        private void ConfigureSerialisers(IServiceCollection services)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            var formatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = settings
            };

            var serialiser = new SerializerBuilder()
                                 .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                 .Build();
            var deserialiser = new DeserializerBuilder()
                                   .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                   .Build();

            services.AddSingleton<JsonSerializerSettings>(settings);
            services.AddSingleton<JsonMediaTypeFormatter>(formatter);
            services.AddSingleton<ISerializer>(serialiser);
            services.AddSingleton<IDeserializer>(deserialiser);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IArticleService, ArticleService>();
        }

        private void ConfigureHelpers(IServiceCollection services)
        {
            services.AddTransient<IPageCrawlingHelper, PageCrawlingHelper>();
            services.AddTransient<IMarkdownHelper, MarkdownHelper>();

            services.AddSingleton<Regex>(p =>
            {
                var settings = p.GetService<AppSettings>();
                var regex = new Regex(settings.DevTo.Article.IdPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                return regex;
            });
        }
    }
}
