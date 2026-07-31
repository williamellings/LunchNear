using Microsoft.JSInterop;

namespace LunchNear.Web.Services;

public class GeolocationService
{
    private readonly IJSRuntime _js;

    public GeolocationService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<(double Lat, double Lng)?> GetCurrentPositionAsync()
    {
        try
        {
            var result = await _js.InvokeAsync<GeoPosition>("lunchnearGeo.getCurrentPosition");
            return (result.Latitude, result.Longitude);
        }
        catch
        {
            return null;
        }
    }

    private class GeoPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
