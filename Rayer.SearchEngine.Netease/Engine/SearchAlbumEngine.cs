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
            foreach (var item in response.Result.Albums)
            {
                item.Cover += "?param=512y512";
            }

            var domain = Mapper.Map<SearchAlbum>(response);

            return domain;
        }

        return default!;
    }

    public async Task<Album> SearchAlbumDetailAsync(long id)
    {
        var result = await Searcher.GetAsync(
            AlbumSelector.GetAlbum()
                .WithParam("id", id.ToString())
                .Build());

        var response = result.ToEntity<AlbumDetailModel>();

        if (response is not null)
        {
            response.Album.Cover += "?param=512y512";

            foreach (var item in response.Audios)
            {
                if (item.Album is not null)
                {
                    if (!string.IsNullOrEmpty(item.Album.Cover))
                    {
                        item.Album.Cover += "?param=512y512";
                    }
                    else
                    {
                        item.Album.Cover = response.Album.Cover;
                    }
                }
            }

            var domain = Mapper.Map<Album>(response);

            domain.Type = SearchType.Album;

            for (var i = 0; i < domain.Audios.Length; i++)
            {
                var audio = domain.Audios[i];

                if (audio.Album is not null)
                {
                    if (audio.Album.Cover is null)
                    {
                        audio.Album.Cover = domain.Cover;
                    }
                }
            }

            return domain;
        }

        return default!;
    }
}