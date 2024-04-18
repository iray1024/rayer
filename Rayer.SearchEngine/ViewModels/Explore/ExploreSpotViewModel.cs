using CommunityToolkit.Mvvm.ComponentModel;
using Rayer.SearchEngine.Business.Login.Abstractions;

namespace Rayer.SearchEngine.ViewModels.Explore;

public partial class ExploreSpotViewModel : ObservableObject
{
    private readonly ILoginManager _loginManager;

    public ExploreSpotViewModel(ILoginManager loginManager)
    {
        _loginManager = loginManager;
    }

    public async Task OnLoadAsync()
    {

    }
}