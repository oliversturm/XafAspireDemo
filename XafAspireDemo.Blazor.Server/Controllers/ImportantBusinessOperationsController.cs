using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        SimpleAction importantBusinessAction;
        IServiceProvider serviceProvider;

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

            // This is where we perform the magic for the important business action.

            importantBusinessAction.Enabled["ImportantBusinessActionRunning"] = false;

            using var activity = telemetry.ActivitySource.StartActivity("ImportantBusinessAction");
            logger.LogInformation("ImportantBusinessAction started.");

            try
            {
                // Run a task that waits a random time between half a second and five seconds.
                await Task.Run(() =>
                {
                    Thread.Sleep(new Random().Next(500, 5000));
                });
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

                // And then we use the meter to count the number of times we've done that.
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
}
