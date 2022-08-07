using PROJBP.Service;
using Quartz;

namespace PROJBP.UI.Modules
{

    [DisallowConcurrentExecution]
    public class ReadAndPushBillNoJob : IJob
    {
        private readonly ILogger<ReadAndPushBillNoJob> _logger;
        private readonly IConfiguration _configuration;
        IBillService _billService;
        public ReadAndPushBillNoJob(ILogger<ReadAndPushBillNoJob> logger, IBillService billService, IConfiguration configuration)
        {
            _logger = logger;
            _billService = billService;
            _configuration = configuration;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var urlKey = $"eBILL_Scheduler:BaseUrl";
            var tokenKey = $"eBILL_Scheduler:Token";

            string _baseUrl = _configuration[urlKey].ToString();
            string _token = _configuration[tokenKey].ToString();

            try
            {
                _billService.ProcessBillsDuringOfficeHoursAsync(_baseUrl: _baseUrl, _token: _token);
                //do something other than getting the data;
             }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            

            return Task.CompletedTask;
        }
    }

    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            IConfiguration config)
            where T : IJob
        {
            // Use the name of the IJob as the appsettings.json key
            string jobName = typeof(T).Name;

            // Try and load the schedule from configuration
            var configKey = $"eBILL_Scheduler:{jobName}";
            var cronSchedule = config[configKey];

            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "-trigger")
                .WithCronSchedule(cronSchedule)); // use the schedule from configuration
        }
    }


}