using Rayer.FrameworkCore;
using Rayer.SearchEngine.Core.Abstractions.Provider;
using Rayer.SearchEngine.Netease.Models;
using Rayer.SearchEngine.Netease.Models.Search.Audio;

namespace Rayer.SearchEngine.Netease.Extensions;

internal static class CopyrightExtensions
{
    public static bool Playable(this SearchAudioDetailInformationModel detail, PrivilegesModel privilege, out string reason)
    {
        var loginManager = AppCore.GetRequiredService<IAggregationServiceProvider>().LoginManager;

        reason = string.Empty;

        if (privilege.Pl > 0)
        {
            return true;
        }

        if (detail.Fee == 1 || privilege.Fee == 1)
        {
            if (loginManager.Account.Profile is not null &&
                loginManager.Account.Account.VipType == 11)
            {
                return true;
            }
            else
            {
                reason = "仅限VIP";

                return false;
            }
        }

        else if (detail.Fee == 4 || privilege.Fee == 4)
        {
            reason = "付费专辑";

            return false;
        }
        else if (detail.NoCopyright is not null)
        {
            reason = "无版权";

            return false;
        }
        else if (privilege.St < 0 && loginManager.Account.Profile is not null)
        {
            reason = "已下架";

            return false;
        }

        return false;
    }

    public static bool Playable(this SearchAudioDetailInformationModel detail, out string reason)
    {
        var loginManager = AppCore.GetRequiredService<IAggregationServiceProvider>().LoginManager;

        reason = string.Empty;

        if (detail.Fee == 1)
        {
            if (loginManager.Account.Profile is not null &&
                loginManager.Account.Account.VipType == 11)
            {
                return true;
            }
            else
            {
                reason = "仅限VIP";

                return false;
            }
        }

        else if (detail.Fee == 4)
        {
            reason = "付费专辑";

            return false;
        }
        else if (detail.NoCopyright is not null)
        {
            reason = "无版权";

            return false;
        }

        return false;
    }
}