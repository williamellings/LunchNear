using Microsoft.JSInterop;

namespace LunchNear.Web.Services;

public class UserIdentityService
{
    private const string StorageKey = "lunchnear-user-id";
    private readonly IJSRuntime _js;
    private string? _userId;

    public UserIdentityService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string> GetUserIdAsync()
    {
        if (_userId != null)
        {
            return _userId;
        }

        _userId = await _js.InvokeAsync<string?>("lunchnearGeo.getOrCreateUserId");
        return _userId ?? Guid.NewGuid().ToString();
    }
}
