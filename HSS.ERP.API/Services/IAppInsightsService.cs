using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace HSS.ERP.API.Services
{
    public interface IAppInsightsService
    {
        void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null);
        void TrackException(Exception exception, Dictionary<string, string>? properties = null);
        void TrackPageView(string pageName, string? userId = null, Dictionary<string, string>? properties = null);
        void TrackUserAction(string action, string? userId = null, Dictionary<string, string>? properties = null);
        void TrackApiCall(string apiEndpoint, TimeSpan duration, bool success, string? userId = null);
        void TrackDatabaseQuery(string operation, TimeSpan duration, bool success, string? additionalInfo = null);
        void TrackTeamsAction(string action, string? userId = null, string? teamId = null, string? channelId = null);
    }
}
