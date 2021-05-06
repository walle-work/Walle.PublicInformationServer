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
            var url = $"https://restapi.amap.com/v5/ip?ip={address}&type={v}&key={key}";
            HttpClient client = new HttpClient();
            var respStr = await client.GetStringAsync(url);
            RespIpInfoModel resp = respStr.ToObject<RespIpInfoModel>();
            return resp;
        }

        public class RespIpInfoModel
        {
            public string Status { get; set; } = string.Empty;
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
