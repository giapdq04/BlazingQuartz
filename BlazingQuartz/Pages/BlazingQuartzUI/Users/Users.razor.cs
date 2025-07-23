using BlazeQuartz.Components;
using BlazeQuartz.Core.Enums;
using BlazeQuartz.Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazeQuartz.Pages.BlazingQuartzUI.Users
{
    public partial class Users : ComponentBase
    {
        private MudTable<User> table;
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

        private async Task<TableData<User>> LoadServerData(TableState state)
        {
            try
            {
                var result = await AuthService.GetAllUsersAsync(state.Page, state.PageSize, searchTerm);
                return new TableData<User> { Items = result.users, TotalItems = result.totalCount };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading users: {ex.Message}", Severity.Error);
                return new TableData<User> { Items = Enumerable.Empty<User>(), TotalItems = 0 };
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
                    if (table != null)
                    {
                        await table.ReloadServerData();
                    }
                });
            };
            _debounceTimer.AutoReset = false;
            _debounceTimer.Start();
        }

        private async Task AddUser(User user)
        {
            try
            {
                var result = await AuthService.AddUser(user);
                if (result)
                {
                    Snackbar.Add($"Successfully added user '{user.Username}'", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Cannot add user '{user.Username}'. Please try again.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error adding user: {ex.Message}", Severity.Error);
            }
        }

        private async Task EditUser(User user)
        {
            try
            {
                var result = await AuthService.EditUser(user);
                if (result)
                {
                    Snackbar.Add($"Successfully updated user '{user.Username}'", Severity.Success);
                }
                else
                {
                    Snackbar.Add($"Cannot update user '{user.Username}'. Please try again.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error updating user: {ex.Message}", Severity.Error);
            }
        }

        private async Task DeleteUser(User user)
        {
            var result = await AuthService.DeleteUser(user.UserId);
            if (result)
            {
                Snackbar.Add($"Successfully deleted user '{user.Username}'", Severity.Success);
            }
            else
            {
                Snackbar.Add($"Cannot delete user '{user.Username}'. Please try again.", Severity.Error);
            }
            await table.ReloadServerData();
        }

        private async Task OpenRoleManagerDialog()
        {
            try
            {
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
                var dialog = DialogService.Show<RoleDialog>("Role Management", options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    //var editUser = (User)result.Data;
                    //await AddUser(editUser);
                    //await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening edit dialog: {ex.Message}");
            }
        }

        private async Task OpenCreateDialog()
        {
            try
            {
                var parameters = new DialogParameters();
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<EditUserDialog>("Create User", parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    var editUser = (User)result.Data;
                    await AddUser(editUser);
                    await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening edit dialog: {ex.Message}");
            }
        }

        private async Task OpenEditDialog(User user)
        {
            try
            {
                var parameters = new DialogParameters { { "User", user } };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<EditUserDialog>("Edit User", parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    var editUser = (User)result.Data;
                    await EditUser(editUser);
                    await table.ReloadServerData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening edit dialog: {ex.Message}");
            }
        }

        private async Task ConfirmDeleteUser(User user)
        {
            try
            {
                var parameters = new DialogParameters
            {
                { "ContentText", $"Bạn có chắc chắn muốn xóa user '{user.Username}'?" },
                { "ButtonText", "Xóa" },
                { "Color", Color.Error }
            };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var dialog = DialogService.Show<ConfirmDialog>("Xác nhận xóa", parameters, options);
                var result = await dialog.Result;

                if (!result.Cancelled)
                {
                    await DeleteUser(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming delete: {ex.Message}");
            }
        }
    }
}

