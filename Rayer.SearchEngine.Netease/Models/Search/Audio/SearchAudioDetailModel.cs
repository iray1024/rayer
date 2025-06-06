﻿using Rayer.SearchEngine.Core.Http.Abstractions;

namespace Rayer.SearchEngine.Netease.Models.Search.Audio;

public class SearchAudioDetailModel : ResponseBase
{
    [JsonPropertyName("songs")]
    public SearchAudioDetailInformationModel[] Details { get; set; } = [];

    public PrivilegesModel[] Privileges { get; set; } = [];
}