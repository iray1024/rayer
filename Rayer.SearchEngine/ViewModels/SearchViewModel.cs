using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.SearchEngine.Models.Response.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rayer.SearchEngine.ViewModels;

public partial class SearchViewModel : ObservableObject
{


    public SearchAggregationModel Model { get; set; } = null!;
}