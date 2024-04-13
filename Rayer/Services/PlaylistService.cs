using Rayer.Core.Framework.Injection;
using Rayer.Core.PlayControl.Abstractions;

namespace Rayer.Services;

[Inject<IPlaylistService>]
internal class PlaylistService : IPlaylistService
{

}