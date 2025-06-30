using BlazeQuartz.Services;
using Microsoft.AspNetCore.Components;

namespace BlazeQuartz.Shared;

public partial class MainLayout
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private LayoutService LayoutService { get; set; } = null!;
    private bool _loading = true;

    private string GetActiveClass(BasePage page)
    {
        return page == LayoutService.GetDocsBasePage(NavigationManager.Uri) ? "nav-item light-blue darken-1 mx-1 px-3" : "nav-item mx-1 px-3";
    }


    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(100);
        _loading = false;
    }

    private async Task Logout()
    {
        await authStateProvider.MarkUserAsLoggedOut();
    }
}



