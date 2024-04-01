using Rayer.Core.Models;

namespace Rayer.Core.Abstractions;

public interface IThemeResourceProvider
{
    PlaybarResource GetPlaybarResource();
}