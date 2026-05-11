using System.Net;
using System.Net.Sockets;

namespace Common.Core.Jwt;

public static class IpUtilities
{
    public static string GetIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }

        return string.Empty;
    }
}
