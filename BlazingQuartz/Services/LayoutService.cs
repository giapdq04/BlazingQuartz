using System;
namespace BlazeQuartz.Services
{
    public class LayoutService
    {
        public LayoutService()
        {
        }

        public BasePage GetDocsBasePage(string uri)
        {
            if (uri.Contains("/overview"))
            {
                return BasePage.Overview;
            }
            else if (uri.Contains("/schedules"))
            {
                return BasePage.Schedules;
            }
            else if (uri.Contains("/triggers"))
            {
                return BasePage.Triggers;
            }
            else if (uri.Contains("/history"))
            {
                return BasePage.History;
            }
            else if (uri.Contains("/calendars"))
            {
                return BasePage.Calendars;
            }
            else if (uri.Contains("/fileupload"))
            {
                return BasePage.FileUpload;
            }
            else if (uri.Contains("/logviewer"))
            {
                return BasePage.LogViewer;
            }
            else if (uri.Contains("/login"))
            {
                return BasePage.Login;
            }
            else
            {
                return BasePage.None;
            }
        }
    }
}

