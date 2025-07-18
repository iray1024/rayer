﻿#nullable disable
using Rayer.Core.Lyric.Enums;
using Rayer.SearchEngine.Lyric.Abstractions;
using Rayer.SearchEngine.Lyric.Models;

namespace Rayer.SearchEngine.Lyric.Providers.Web.Kugou;

public class SearchSongResponse
{
    public int Status { get; set; }

    public string Error { get; set; }

    public DataItem Data { get; set; }

    public class DataItem
    {
        public int Timestamp { get; set; }

        public int Total { get; set; }

        public List<InfoItem> Info { get; set; }

        public class InfoItem
        {
            public string Hash { get; set; }

            [JsonPropertyName("songname")]
            public string SongName { get; set; }

            [JsonPropertyName("album_name")]
            public string AlbumName { get; set; }

            [JsonPropertyName("songname_original")]
            public string SongNameOriginal { get; set; }

            [JsonPropertyName("singername")]
            public string SingerName { get; set; }

            public int Duration { get; set; }

            [JsonPropertyName("filename")]
            public string Filename { get; set; }

            public List<InfoItem> Group { get; set; }
        }
    }

    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }
}

public class SearchLyricsResponse
{
    public int Status { get; set; }

    public string Info { get; set; }

    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("errmsg")]
    public string ErrorMessage { get; set; }

    public string Keywork { get; set; }

    public string Proposal { get; set; }

    [JsonPropertyName("has_complete_right")]
    public int HasCompleteRight { get; set; }

    public int Ugc { get; set; }

    [JsonPropertyName("ugccount")]
    public int UgcCount { get; set; }

    public int Expire { get; set; }

    public List<Candidate> Candidates { get; set; }

    public class Candidate
    {
        public string Id { get; set; }

        [JsonPropertyName("product_from")]
        public string ProductFrom { get; set; }

        [JsonPropertyName("accesskey")]
        public string AccessKey { get; set; }

        [JsonPropertyName("can_score")]
        public bool CanScore { get; set; }

        public string Singer { get; set; }

        public string Song { get; set; }

        public int Duration { get; set; }

        public string Uid { get; set; }

        public string Nickname { get; set; }

        public string Origiuid { get; set; }

        public string Originame { get; set; }

        public string Transuid { get; set; }

        public string Transname { get; set; }

        public string Sounduid { get; set; }

        public string Soundname { get; set; }

        public string Language { get; set; }

        [JsonPropertyName("krctype")]
        public int KrcType { get; set; }

        public int Hitlayer { get; set; }

        public int Hitcasemask { get; set; }

        public int Adjust { get; set; }

        public int Score { get; set; }

        [JsonPropertyName("contenttype")]
        public int ContentType { get; set; }

        [JsonPropertyName("content_format")]
        public int ContentFormat { get; set; }

        [JsonPropertyName("trans_id")]
        public string TransId { get; set; }
    }
}

public class LyricResult : ILyricResult
{
    public int Status { get; set; }

    public string Id { get; set; }

    public string Info { get; set; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; set; }

    public int ContentType { get; set; }

    [JsonPropertyName("_source")]
    public string Source { get; set; }

    public string Charset { get; set; }

    public string Content { get; set; }

    public string KrcContent { get; set; }

    public LyricWrapper GetLyric()
    {
        return new LyricWrapper
        {
            Lyric = !string.IsNullOrEmpty(Content) ? Encoding.UTF8.GetString(Convert.FromBase64String(Content)) : string.Empty,
            Klyric = KrcContent
        };
    }

    public (string, LyricRawType) GetLyricTarget()
    {
        if (!string.IsNullOrEmpty(KrcContent))
        {
            return (KrcContent, LyricRawType.Krc);
        }

        return (GetLyric().Lyric, LyricRawType.Lrc);
    }
}
#nullable restore