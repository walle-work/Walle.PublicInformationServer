using IPTools.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Walle.Components.Extensions;
using Walle.Components.Responses;

namespace Walle.PublicInformationServer.Controllers
{
    public class IPAddressController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("/api/ipaddress")]
        public RespModel<IpInfo> GetIpAddress([FromQuery] string ip)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = HttpContext.Connection.RemoteIpAddress.ToString();
                }
                if (ip.IsNotNull())
                {
                    var ipinfo = IpTool.Search(ip);
                    if (ipinfo.IsNotNull())
                    {
                        return new RespModel<IpInfo>
                        {
                            Data = ipinfo,
                            IsOk = true,
                            Message = "query ip info ok."
                        };
                    }
                    else
                    {
                        return new RespModel<IpInfo>
                        {

                            Data = new IpInfo
                            {
                                IpAddress = ip
                            },
                            IsOk = true,
                            Message = "get ip info ok, but no local info."
                        };
                    }
                }
                else
                {
                    return new RespModel<IpInfo>
                    {

                        Data = new IpInfo(),
                        IsOk = false,
                        Message = "get ip info failure."
                    };
                }
            }
            catch (Exception ex)
            {
                return new RespModel<IpInfo>
                {
                    IsOk = false,
                    Message = ex.Message
                };
            }
        }
    }
}
