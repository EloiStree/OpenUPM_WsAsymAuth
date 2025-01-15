

using System;
namespace Eloi.WsMetaMaskAuth
{
    public class MetaMaskIIDUtility
{

    public static void GetCurrentNtpTimeNowUTC(out long timeInTick)
    {
        GetTimestampNowUTCNTP(0, out timeInTick);
    }
    static void GetTimestampNowUTCNTP(long offset, out long timeInTick)
    {
        timeInTick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
}
