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
        public static Dictionary<string, RespIpInfoModel> IpInfoCache = new Dictionary<string, RespIpInfoModel>();
      
        [HttpGet("/api/ip")]
        public async Task<RespIpInfoModel> GetIpInfoAsync([FromQuery] string ip)
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
            if (!IpInfoCache.TryGetValue(url, out RespIpInfoModel resp))
            {
                HttpClient client = new HttpClient();
                var respStr = await client.GetStringAsync(url);
                resp = respStr.ToObject<RespIpInfoModel>();
                resp.Ip = ip;
                if (resp.Status != 0)
                {
                    IpInfoCache.Add(url, resp);
                }
            }
            return resp;
        }

        public class RespIpInfoModel
        {
            public int Status { get; set; } = 0;
            public string Info { get; set; } = string.Empty;
            public string Infocode { get; set; } = string.Empty;

            public string Country { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Adcode { get; set; } = string.Empty;
            public string Rectangle { get; set; } = string.Empty;
            public string District { get; set; } = string.Empty;
            public string Isp { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Ip { get; set; } = string.Empty;
        }
    }
}
