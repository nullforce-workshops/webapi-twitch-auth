using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace WebApiTwitchAuth.Twitch
{
    public class TwitchAuthenticationOptions : OAuthOptions
    {
        public bool ForceVerify { get; set; }

        public TwitchAuthenticationOptions()
        {
            ClaimsIssuer = "Twitch";
            CallbackPath = "/auth/twitch/callback";

            AuthorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
            TokenEndpoint = "https://id.twitch.tv/oauth2/token";
            UserInformationEndpoint = "https://api.twitch.tv/helix/users/";

            Scope.Add("user:read:email");

            ClaimActions.MapCustomJson(ClaimTypes.NameIdentifier, user => GetData(user, "id"));
            ClaimActions.MapCustomJson(ClaimTypes.Name, user => GetData(user, "login"));
            ClaimActions.MapCustomJson(ClaimTypes.Email, user => GetData(user, "email"));

            ClaimActions.MapCustomJson("urn:twitch:displayname", user => GetData(user, "display_name"));
            ClaimActions.MapCustomJson("urn:twitch:type", user => GetData(user, "type"));
            ClaimActions.MapCustomJson("urn:twitch:broadcastertype", user => GetData(user, "broadcaster_type"));
            ClaimActions.MapCustomJson("urn:twitch:description", user => GetData(user, "description"));
            ClaimActions.MapCustomJson("urn:twitch:profileimageurl", user => GetData(user, "profile_image_url"));
            ClaimActions.MapCustomJson("urn:twitch:offlineimageurl", user => GetData(user, "offline_image_url"));
        }

        private static string GetData(JsonElement user, string key)
        {
            if (!user.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            return data.EnumerateArray().Select((p) => p.GetString(key)).FirstOrDefault();
        }
    }
}
