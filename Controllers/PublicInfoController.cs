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
        public static Dictionary<string, IpInfo> IpInfoCache = new Dictionary<string, IpInfo>();

        [HttpGet("/api/ip")]
        public RespModel<IpInfo> GetIpInfoAsync([FromQuery] string ip)
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
            if (!IpInfoCache.TryGetValue(url, out IpInfo info))
            {
                HttpClient client = new HttpClient();
                var respStr = client.GetStringAsync(url).Result;
                var resp = respStr.ToObject<AmapResp>();
                if (resp.Status != 0)
                {
                    info = respStr.ToObject<IpInfo>();
                    IpInfoCache.Add(url, info);
                }
            }
            if (info == null) {
                info = new IpInfo();
            }
            info.Ip = address.ToString();
            return new RespModel<IpInfo>
            {
                Data = info,
                IsOk = true
            };
        }

        public class AmapResp
        {
            public int Status { get; set; } = 0;
            public string Info { get; set; } = string.Empty;
            public string Infocode { get; set; } = string.Empty;

        }

        public class IpInfo
        {
            /// <summary>
            /// 国家（或地区），中文
            /// </summary>
            public string Country { get; set; } = "中国";

            /// <summary>
            /// 省（二级），中文
            /// </summary>
            public string Province { get; set; } = string.Empty;

            /// <summary>
            /// 市（三级），中文
            /// </summary>
            public string City { get; set; } = string.Empty;

            /// <summary>
            /// 区（四级），中文
            /// </summary>
            public string District { get; set; } = string.Empty;

            /// <summary>
            /// 国家基础地理信息中心定义的区域代码
            /// </summary>
            public string Adcode { get; set; } = string.Empty;

            /// <summary>
            /// 所在城市矩形区域范围 所在城市范围的左下右上对标对
            /// </summary>
            public string Rectangle { get; set; } = string.Empty;

            /// <summary>
            /// 运营商 如电信、联通、移动
            /// </summary>
            public string Isp { get; set; } = string.Empty;

            /// <summary>
            /// 经纬度 精度在前，纬度在后，格式：X,Y
            /// </summary>
            public string Location { get; set; } = string.Empty;

            /// <summary>
            /// IP地址 提交的 Ipv4/ Ipv6地址。
            /// </summary>
            public string Ip { get; set; } = string.Empty;
        }
    }
}
