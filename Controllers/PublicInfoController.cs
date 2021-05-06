using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Walle.Components.Extensions;
using Walle.Components.Responses;

namespace Walle.PublicInformationServer.Controllers
{
    [AllowAnonymous]
    public class PublicInfoController : ControllerBase
    {
        public static Dictionary<string, RespIpInfoV3Model> RespIpInfoCacheV3 = new Dictionary<string, RespIpInfoV3Model>();
        public static Dictionary<string, RespIpInfoV5Model> RespIpInfoCacheV5 = new Dictionary<string, RespIpInfoV5Model>();

        [HttpGet("/api/ip/v3")]
        public async Task<RespIpInfoV3Model> GetIpInfoAsync([FromQuery] string ip)
        {

            IPAddress address;
            if (string.IsNullOrWhiteSpace(ip))
            {
                address = HttpContext.Connection.RemoteIpAddress;
            }
            else if (!IPAddress.TryParse(ip, out address))
            {
                return null;
            }
            int v = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 4 : 6;
            var key = "f6db5d357d57ea11f56384c515390093";
            var url = $"https://restapi.amap.com/v3/ip?ip={address}&type={v}&key={key}";
            if (!RespIpInfoCacheV3.TryGetValue(url, out RespIpInfoV3Model resp))
            {
                HttpClient client = new HttpClient();
                var respStr = await client.GetStringAsync(url);
                resp = respStr.ToObject<RespIpInfoV3Model>();
                if (resp.Status != 0)
                {
                    RespIpInfoCacheV3.Add(url, resp);
                }
            }
            return resp;
        }

        [HttpGet("/api/ip/v5")]
        public async Task<RespIpInfoV5Model> GetIpInfoV5Async([FromQuery] string ip)
        {

            IPAddress address;
            if (string.IsNullOrWhiteSpace(ip))
            {
                address = HttpContext.Connection.RemoteIpAddress;
            }
            else if (!IPAddress.TryParse(ip, out address))
            {
                return null;
            }
            int v = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 4 : 6;
            var key = "f6db5d357d57ea11f56384c515390093";
            var url = $"https://restapi.amap.com/v5/ip?ip={address}&type={v}&key={key}";
            if (!RespIpInfoCacheV5.TryGetValue(url, out RespIpInfoV5Model resp))
            {
                HttpClient client = new HttpClient();
                var respStr = await client.GetStringAsync(url);
                resp = respStr.ToObject<RespIpInfoV5Model>();
                if (resp.Status != 0)
                {
                    RespIpInfoCacheV5.Add(url, resp);
                }
            }
            return resp;
        }

        public class RespIpInfoV3Model
        {
            public int Status { get; set; } = 0;
            public string Info { get; set; } = string.Empty;
            public string Infocode { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Adcode { get; set; } = string.Empty;
            public string Rectangle { get; set; } = string.Empty;
        }

        public class RespIpInfoV5Model
        {
            public int Status { get; set; } = 0;
            public string Info { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string District { get; set; } = string.Empty;
            public string Isp { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Ip { get; set; } = string.Empty;
        }
    }
}
