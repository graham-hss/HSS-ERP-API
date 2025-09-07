using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace HSS.ERP.API.Services.Implementations
{
    public class AppInsightsService : IAppInsightsService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<AppInsightsService> _logger;

        public AppInsightsService(TelemetryClient telemetryClient, ILogger<AppInsightsService> logger)
        {
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        public void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null)
        {
            try
            {
                _telemetryClient.TrackEvent(eventName, properties, metrics);
                _logger.LogInformation("Tracked event: {EventName}", eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track event: {EventName}", eventName);
            }
        }

        public void TrackException(Exception exception, Dictionary<string, string>? properties = null)
        {
            try
            {
                _telemetryClient.TrackException(exception, properties);
                _logger.LogError(exception, "Tracked exception: {ExceptionMessage}", exception.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track exception");
            }
        }

        public void TrackPageView(string pageName, string? userId = null, Dictionary<string, string>? properties = null)
        {
            try
            {
                var pageViewTelemetry = new PageViewTelemetry(pageName);
                
                if (!string.IsNullOrEmpty(userId))
                {
                    pageViewTelemetry.Context.User.Id = userId;
                }

                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        pageViewTelemetry.Properties[prop.Key] = prop.Value;
                    }
                }

                _telemetryClient.TrackPageView(pageViewTelemetry);
                _logger.LogInformation("Tracked page view: {PageName} for user: {UserId}", pageName, userId ?? "anonymous");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track page view: {PageName}", pageName);
            }
        }

        public void TrackUserAction(string action, string? userId = null, Dictionary<string, string>? properties = null)
        {
            try
            {
                var eventProperties = new Dictionary<string, string>
                {
                    ["action"] = action,
                    ["actionType"] = "userAction"
                };

                if (!string.IsNullOrEmpty(userId))
                {
                    eventProperties["userId"] = userId;
                }

                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        eventProperties[prop.Key] = prop.Value;
                    }
                }

                _telemetryClient.TrackEvent("UserAction", eventProperties);
                _logger.LogInformation("Tracked user action: {Action} for user: {UserId}", action, userId ?? "anonymous");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track user action: {Action}", action);
            }
        }

        public void TrackApiCall(string apiEndpoint, TimeSpan duration, bool success, string? userId = null)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["endpoint"] = apiEndpoint,
                    ["success"] = success.ToString(),
                    ["actionType"] = "apiCall"
                };

                if (!string.IsNullOrEmpty(userId))
                {
                    properties["userId"] = userId;
                }

                var metrics = new Dictionary<string, double>
                {
                    ["duration"] = duration.TotalMilliseconds
                };

                _telemetryClient.TrackEvent("ApiCall", properties, metrics);
                _logger.LogInformation("Tracked API call: {Endpoint} - Success: {Success}, Duration: {Duration}ms", 
                    apiEndpoint, success, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track API call: {Endpoint}", apiEndpoint);
            }
        }

        public void TrackDatabaseQuery(string operation, TimeSpan duration, bool success, string? additionalInfo = null)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["operation"] = operation,
                    ["success"] = success.ToString(),
                    ["actionType"] = "databaseQuery"
                };

                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    properties["additionalInfo"] = additionalInfo;
                }

                var metrics = new Dictionary<string, double>
                {
                    ["duration"] = duration.TotalMilliseconds
                };

                _telemetryClient.TrackEvent("DatabaseQuery", properties, metrics);
                _logger.LogInformation("Tracked database query: {Operation} - Success: {Success}, Duration: {Duration}ms", 
                    operation, success, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track database query: {Operation}", operation);
            }
        }

        public void TrackTeamsAction(string action, string? userId = null, string? teamId = null, string? channelId = null)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    ["action"] = action,
                    ["actionType"] = "teamsAction"
                };

                if (!string.IsNullOrEmpty(userId))
                {
                    properties["userId"] = userId;
                }

                if (!string.IsNullOrEmpty(teamId))
                {
                    properties["teamId"] = teamId;
                }

                if (!string.IsNullOrEmpty(channelId))
                {
                    properties["channelId"] = channelId;
                }

                _telemetryClient.TrackEvent("TeamsAction", properties);
                _logger.LogInformation("Tracked Teams action: {Action} for user: {UserId} in team: {TeamId}", 
                    action, userId ?? "anonymous", teamId ?? "unknown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track Teams action: {Action}", action);
            }
        }
    }
}
