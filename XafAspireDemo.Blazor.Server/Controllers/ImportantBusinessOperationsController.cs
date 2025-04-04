using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using OpenTelemetry.Trace;

namespace XafAspireDemo.Blazor.Server.Controllers
{
    public partial class ImportantBusinessOperationsController : Controller
    {
        readonly SimpleAction importantBusinessAction;
        readonly IServiceProvider serviceProvider;

        public ImportantBusinessOperationsController()
        {
            importantBusinessAction = new SimpleAction(
                this,
                "ImportantBusinessAction",
                PredefinedCategory.View
            );
            importantBusinessAction.Execute += ImportantBusinessAction_Execute;
        }

        [ActivatorUtilitiesConstructor]
        public ImportantBusinessOperationsController(IServiceProvider serviceProvider)
            : this()
        {
            this.serviceProvider = serviceProvider;
        }

        private async void ImportantBusinessAction_Execute(
            object sender,
            SimpleActionExecuteEventArgs e
        )
        {
            var telemetry = serviceProvider.GetRequiredService<Telemetry>();
            var logger = serviceProvider.GetRequiredService<
                ILogger<ImportantBusinessOperationsController>
            >();

            importantBusinessAction.Enabled["ImportantBusinessActionRunning"] = false;

            using var activity = telemetry.ActivitySource.StartActivity("ImportantBusinessAction");
            logger.LogInformation("ImportantBusinessAction started.");

            try
            {
                // This is where we perform the magic for the important business action.
                // Run a task that waits a random time between half a second and five seconds.
                await Task.Run(() =>
                {
                    Thread.Sleep(new Random().Next(500, 5000));
                });

                // Call the demo service endpoint to get an important business value
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var response = await httpClient.GetFromJsonAsync<ImportantBusinessValueResponse>(
                    "https://demoservice/important-business-value"
                );
                logger.LogInformation(
                    "Received important business value from service: {ImportantBusinessValue}",
                    response?.ImportantBusinessValue
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ImportantBusinessAction failed.");
                activity?.SetStatus(ActivityStatusCode.Error);
                activity?.AddException(ex);
                throw;
            }
            finally
            {
                activity?.Stop();

                importantBusinessAction.Enabled["ImportantBusinessActionRunning"] = true;

                // And then we use the meters to count the number of times we've done that, and how long it took
                telemetry.ImportantBusinessOperationCounter.Add(1);
                telemetry.ImportantBusinessOperationDuration.Record(
                    activity.Duration.TotalMilliseconds
                );
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            var logger = serviceProvider.GetRequiredService<
                ILogger<ImportantBusinessOperationsController>
            >();
            logger.LogInformation("ImportantBusinessOperationsController activated.");
        }

        protected override void OnDeactivated()
        {
            var logger = serviceProvider.GetRequiredService<
                ILogger<ImportantBusinessOperationsController>
            >();
            logger.LogInformation("ImportantBusinessOperationsController deactivated.");

            base.OnDeactivated();
        }
    }

    // Response model for the important business value endpoint
    public class ImportantBusinessValueResponse
    {
        [JsonPropertyName("importantBusinessValue")]
        public int ImportantBusinessValue { get; set; }
    }
}
