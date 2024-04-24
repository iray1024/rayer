using Rayer.Core.Common;
using Rayer.Core.Framework.Injection;
using Rayer.SearchEngine.Core.Abstractions;
using Rayer.SearchEngine.Core.Domain.Album;
using Rayer.SearchEngine.Core.Enums;
using Rayer.SearchEngine.Netease.Models.Search.Album;

namespace Rayer.SearchEngine.Netease.Engine;

[Inject<ISearchAlbumEngine>(ServiceKey = SearcherType.Netease)]
internal class SearchAlbumEngine : SearchEngineBase, ISearchAlbumEngine
{
    public async Task<SearchAlbum> SearchAsync(string keywords, int offset)
    {
        var result = await Searcher.GetAsync(
            SearchSelector.SampleSearch()
                .WithParam("keywords", keywords)
                .WithParam("type", "10")
                .WithParam("offset", offset.ToString())
                .Build());

        var response = result.ToEntity<SearchAlbumModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<SearchAlbum>(response);

            return domain;
        }

        return default!;
    }

    public async Task<Album> SearchFavoriteAlbumListAsync(long id)
    {
        var result = await Searcher.GetAsync(
            AlbumSelector.GetAlbum()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<AlbumDetailModel>();

        if (response is not null)
        {
            var domain = Mapper.Map<Album>(response);

            domain.Type = SearchType.Album;
            domain.Cover += "?param=512y512";

            for (var i = 0; i < domain.Audios.Length; i++)
            {
                var audio = domain.Audios[i];

                if (audio.Album is not null)
                {
                    if (string.IsNullOrEmpty(audio.Album.Cover))
                    {
                        audio.Album.Cover = domain.Cover;
                    }
                    else
                    {
                        audio.Album.Cover += "?param=512y512";
                    }
                }
            }

            return domain;
        }

        return default!;
    }
}