using Rayer.Core;
using Rayer.SearchEngine.Business.Login.Abstractions;
using Rayer.SearchEngine.Models.Domian;

namespace Rayer.SearchEngine.Extensions;

internal static class SearchAudioExtensions
{
    public static bool Playable(this AudioDetail detail)
    {
        var loginManager = AppCore.GetRequiredService<ILoginManager>();

        var privilege = detail.Privilege;

        if (privilege.Pl > 0)
        {
            return true;
        }

        if (detail.Fee == 1 || privilege.Fee == 1)
        {
            if (loginManager.AccountInfo.Profile is not null &&
                loginManager.AccountInfo.Account.VipType == 11)
            {
                return true;
            }
            else
            {
                detail.NonePlayableReason = "仅限VIP";

                return false;
            }
        }

        else if (detail.Fee == 4 || privilege.Fee == 4)
        {
            detail.NonePlayableReason = "付费专辑";

            return false;
        }
        else if (detail.NoCopyright is not null)
        {
            detail.NonePlayableReason = "无版权";

            return false;
        }
        else if (privilege.St < 0 && loginManager.AccountInfo.Profile is not null)
        {
            detail.NonePlayableReason = "已下架";

            return false;
        }

        return false;
    }
}