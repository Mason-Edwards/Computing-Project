using Microsoft.JSInterop;
using Dashboard.Shared.Models;

namespace Dashboard.Client.Services
{
    public static class ChartService
    {

        // Methods for adding things to chart
        public static async Task AddParameter(string parameter, IJSRuntime JSRuntime)
        {
            if (JSRuntime == null) throw new ArgumentException(nameof(JSRuntime));
            await JSRuntime.InvokeAsync<string>("addNewParameter", parameter);
        }

        public static async Task AddParameterData(string parameter, ParameterData parameterData, IJSRuntime JSRuntime)
        {
            if (JSRuntime == null) throw new ArgumentException(nameof(JSRuntime));
            await JSRuntime.InvokeAsync<string>("addData", parameter, parameterData);
        }
    }
}
