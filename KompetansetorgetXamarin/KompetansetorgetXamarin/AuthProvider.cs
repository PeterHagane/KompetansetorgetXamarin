using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin
{
    public static class AuthProvider
    {
        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string Name = "Google";
        public static string ClientId = "976073814766-kpv0giko30e7s8u3gd15s7238a2hqesl.apps.googleusercontent.com";
        //public static string ClientId = "232694463747-kvgavlaaekt3jj6vb934koptl6rsv1q2.apps.googleusercontent.com";
        public static string ClientSecret = "";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://accounts.google.com/o/oauth2/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set this property to the location the user will be redirected too after successfully authenticating
        public static string RedirectUrl = "https://httpbin.org/get";
        public static string Image = "http://icons.iconarchive.com/icons/graphics-vibe/simple-rounded-social/128/google-icon.png";
        public static string ApiRequests = "https://maps.googleapis.com/maps/api/geocode/json?address=122+Flinders+St,+Darlinghurst,+NSW,+Australia&sensor=false";
    }
}
