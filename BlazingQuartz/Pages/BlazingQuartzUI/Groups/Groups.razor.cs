using BlazeQuartz.Components;
using BlazeQuartz.Core.Enums;
using BlazeQuartz.Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
namespace BlazeQuartz.Pages.BlazingQuartzUI.Groups
{
    public partial class Groups : ComponentBase
    {
        private MudTable<UserGroup> table;
        private string searchTerm = "";
        private int rowsPerPage = 10;
        private System.Timers.Timer? _debounceTimer;
        private const int DebounceInterval = 600; // ms
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is { IsAuthenticated: true })
            {
                // Lấy tất cả role
                var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

                if (user.IsInRole(UserRoles.User))
                {
                    Navigation.NavigateTo("/");
                }
            }
        }

        private async Task<TableData<UserGroup>> LoadServerData(TableState state)
        {
            try
            {
                var result = await UserGroupService.GetAllGroup(state.Page, state.PageSize, searchTerm);
                return new TableData<UserGroup> { Items = result.groups, TotalItems = result.totalCount };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading groups: {ex.Message}");
                return new TableData<UserGroup> { Items = Enumerable.Empty<UserGroup>(), TotalItems = 0 };
            }
        }

        private void OnSearchChanged()
        {
            // Nếu timer đang chạy thì dừng lại
            _debounceTimer?.Stop();
            _debounceTimer?.Dispose();

            // Tạo timer mới
            _debounceTimer = new System.Timers.Timer(DebounceInterval);
            _debounceTimer.Elapsed += async (_, __) =>
            {
                _debounceTimer?.Stop();
                _debounceTimer?.Dispose();
                _debounceTimer = null;

                // Reload data trên UI thread
                await InvokeAsync(async () =>
                {
                    await table.ReloadServerData();
                });
            };
            _debounceTimer.AutoReset = false;
            _debounceTimer.Start();
        }

        private async Task OpenCreateDialog()
        {
            try
            {
                var parameters = new DialogParameters();
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<EditGroupDialog>("Create Group", parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    var editGroup = (UserGroup)result.Data;
                    await AddGroup(editGroup);
                    await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening edit dialog: {ex.Message}");
            }
        }

        private async Task AddGroup(UserGroup group)
        {
            try
            {
                var result = await UserGroupService.AddGroup(group);
                if (result)
                {
                    Snackbar.Add($"Group '{group.GROUP_NAME}' has been added successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Cannot add group '{group.GROUP_NAME}'. Please try again.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error adding group: {ex.Message}", Severity.Error);
            }
        }

        private async Task OpenEditDialog(UserGroup group)
        {
            try
            {
                var parameters = new DialogParameters { { "Group", group } };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<EditGroupDialog>("Edit Group", parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    var editGroup = (UserGroup)result.Data;
                    await EditGroup(editGroup);
                    await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening edit dialog: {ex.Message}");
            }
        }

        private async Task EditGroup(UserGroup group)
        {
            try
            {
                var result = await UserGroupService.EditGroup(group);
                if (result)
                {
                    Snackbar.Add($"Group '{group.GROUP_NAME}' has been updated successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Cannot update group '{group.GROUP_NAME}'. Please try again.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error updating group: {ex.Message}", Severity.Error);
            }
        }

        private async Task ConfirmDeleteGroup(UserGroup group)
        {
            try
            {
                var parameters = new DialogParameters
            {
                { "ContentText", $"Are you sure you want to delete group '{group.GROUP_NAME}'?" },
                { "ButtonText", "Xóa" },
                { "Color", Color.Error }
            };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<ConfirmDialog>("Xác nhận xóa", parameters, options);
                var result = await dialog.Result;

                if (!result.Cancelled)
                {
                    await DeleteGroup(group);
                    await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming delete: {ex.Message}");
            }
        }

        private async Task DeleteGroup(UserGroup group)
        {
            try
            {
                var result = await UserGroupService.DeleteGroup(group.ID);
                if (result)
                {
                    Snackbar.Add($"Group '{group.GROUP_NAME}' has been deleted successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Cannot delete group '{group.GROUP_NAME}'. Please try again.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting group: {ex.Message}", Severity.Error);
            }
        }


        private async Task OpenViewMemberDialog(UserGroup group)
        {
            try
            {
                var parameters = new DialogParameters { { "Group", group } };
                var options = new DialogOptions
                {
                    CloseOnEscapeKey = true,
                    FullWidth = true,
                    MaxWidth = MaxWidth.Medium
                };
                var dialog = DialogService.Show<ShowMemberDialog>(null, parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    // var editGroup = (UserGroup)result.Data;
                    // await EditGroup(editGroup);
                    // await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
