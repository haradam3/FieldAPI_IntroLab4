#region Copyright
//
// Copyright (C) 2015-2017 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
// Written by M.Harada
// 
#endregion // Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics; 
// Reuse the Field web services calls
using FieldAPIIntro;
// Added for RestSharp. 
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers; 

namespace FieldAPIWebIntro
{
    public partial class FieldUI : System.Web.UI.Page
    {
        //====================================================
        // WebForm Start/End
        //====================================================
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //==========================================================
        // Login/Logout
        //==========================================================
        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            // Field Login call here. 
            string ticket = Field.Login(TextBoxUserName.Text, TextBoxPassword.Text);

            if (!string.IsNullOrEmpty(ticket))
            {
                // If success, change the button to logout.
                ButtonLogin.Enabled = false;
                ButtonLogout.Enabled = true;

                // Save ticket for this session 
                Session["ticket"] = ticket;
            }

            // Show the request and response in the form. 
            // This is for learning purpose. 
            ShowRequestResponse(); 
        }

        protected void ButtonLogout_Click(object sender, EventArgs e)
        {
            string ticket = Session["ticket"] as string;

            // Here is the main call to Field API. 
            bool result = Field.Logout(ticket);

            Session["ticket"] = "";

            ButtonLogin.Enabled = true;
            ButtonLogout.Enabled = false;

            // For our learning, 
            // show the request and response in the form. 
            ShowRequestResponse();
        }

        //===========================================================
        // Helper Functions 
        //===========================================================
        // Show the request and response in the form.
        // This is for learning purpose.
        protected void ShowRequestResponse()
        {            
            IRestResponse response = Field.m_lastResponse;
            TextBoxRequest.Text = response.ResponseUri.AbsoluteUri;
            LabelStatus.Text = "Status: " + response.StatusCode.ToString();
            TextBoxResponse.Text = RemoveAngleBracket(response.Content);
        }

        // Remove angle brackets ("<" and ">") from the given string.
        // .NET framework does not like "<...>"  
        // e.g., Field response might contain like "<no description>"
        // http://forums.asp.net/t/1235144.aspx?A+potentially+dangerous+Request+Form+value+was+detected+from+the+client
        //
        protected string RemoveAngleBracket(string str)
        {
            string s = str.Replace("<", "");
            s = s.Replace(">", "");
            return s; 
        }

        //============================================================
        // Projects  
        //============================================================
        protected void ButtonProject_Click(object sender, EventArgs e)
        {
            string ticket = Session["ticket"] as string;

            List<Project> proj_list = Field.ProjectNames(ticket);

            ShowRequestResponse();

            if (proj_list == null)
            {
                return;
            }

            // Set up a project list
            proj_list = proj_list.OrderBy(x => x.name).ToList();
            DropDownListProjects.DataSource = proj_list;
            DropDownListProjects.DataTextField = "name";
            DropDownListProjects.DataValueField = "id";
            DropDownListProjects.DataBind(); 
            DropDownListProjects.SelectedIndex = 0;
        }

        // If you don't see this is called, check AutoPostBack="True" property of DropDownList in your .aspx page. 
        // https://stackoverflow.com/questions/4905406/dropdownlists-selectedindexchanged-event-not-firing
        protected void DropDownListProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear issues
            DropDownListIssues.Items.Clear();
            TextBoxNewIssue.Text = ""; 
        }

        //==========================================================
        // Issues Retrieve
        //==========================================================
        protected void ButtonIssue_Click(object sender, EventArgs e)
        {
            string ticket = Session["ticket"] as string;
            string project_id = DropDownListProjects.SelectedItem.Value;

            // Here is the main call to the Field web services API.  
            List<Issue> issue_list = Field.IssueList(ticket, project_id);

            // Show the request and response in the form. 
            // This is for learning purpose. 
            ShowRequestResponse();

            if (issue_list == null || issue_list.Count <= 0)
            {
                return; 
            }

            Session["issueList"] = issue_list;

            // To convert an issue back to a JSON tring 
            JsonSerializer serial = new JsonSerializer();

            // Make an issue list in a string and add to a combobox. 
            DropDownListIssues.Items.Clear(); 
            foreach (Issue issue in issue_list)
            {
                IssueFieldItem issueId = issue.fields.Find(x => x.name.Equals("Identifier"));
                string s = issueId.value + " : " + serial.Serialize(issue);
                DropDownListIssues.Items.Add(s);
            }
            DropDownListIssues.SelectedIndex = 0;

            // Make one issue for creation. This is for later use.  
            Issue tmpIssue = issue_list[0];
            TextBoxNewIssue.Text = MakeTemporaryIssueString(tmpIssue);
        }

        protected void DropDownListIssues_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set a new temporary issue string based on the selected one. 
            List<Issue> issue_list = Session["IssueList"] as List<Issue>; 
            Issue tmpIssue = issue_list[DropDownListIssues.SelectedIndex];
            TextBoxNewIssue.Text = MakeTemporaryIssueString(tmpIssue);
        }

        /// Helper function to compose a temporary issue string for issue creation.
        ///  
        /// Given an issue data, compose a string like below.
        /// This is for creating an issue. 
        /// 
        /// [{
        ///     "temporary_id":"Q45", 
        ///     "fields": [
        ///        {"id":"f--description","value":"Test"}, 
        ///        {"id":"f--issue_type_id", "value":"f498d0f5-0be0-11e2-9694-14f6960d7e4f"} 
        ///     ]
        /// }]
        /// 
        private string MakeTemporaryIssueString(Issue issue)
        {
            string issueString = null;

            // Compose the JSON data for fields values 
            foreach (IssueFieldItem item in issue.fields)
            {
                if (item.value == null) continue; // Skipp when no value. 
                if (item.id.Equals("f--identifier")) continue; // Avoid duplicating the ID

                string s = "{\"id\":\"" + item.id + "\","
                    + "\"value\":\"" + item.value.ToString() + "\"},";

                issueString += s;
            }
            int len = issueString.Length;
            if (len > 0)
            {
                // Removing the last extra ',' 
                issueString = issueString.Remove(len - 1);
            }
            // This is the whole string 
            issueString = "[{\"temporary_id\":\"Tmp001\",\"fields\":["
                + issueString + "]}]";

            return issueString;
        }

        //==========================================================
        // Issues Create
        //==========================================================
        protected void ButtonCreate_Click(object sender, EventArgs e)
        {
            string ticket = HttpContext.Current.Session["ticket"] as string;
            string project_id = DropDownListProjects.SelectedItem.Value;

            // Sample JSON string:  
            // [{
            //     "temporary_id":"Q45", 
            //     "fields": [
            //        {"id":"f--description","value":"Test"}, 
            //        {"id":"f--issue_type_id", "value":"f498d0f5-0be0-11e2-9694-14f6960d7e4f"} 
            //     ]
            // }]

            string issues = TextBoxNewIssue.Text;
            string issue_id = Field.IssueCreate(ticket, project_id, issues);

            ShowRequestResponse();
        }

        //============================================================
        // Make a a chart showing the numbers of issues by status.
        // This is not a part of Field API. 
        // Just to show what you can do with the retrieved data.
        //============================================================
        protected void ButtonReport_Click(object sender, EventArgs e)
        {
            List<Issue> issue_list = Session["issueList"] as List<Issue>;

            // Collect data from the issue list.
            // Count the number of issues for each status. 
            Dictionary<string, int> data = CollectData(issue_list);

            // Clear the chart data 
            Chart1.Series[0].Points.Clear();

            // Fill the chart data  
            foreach (var item in data)
            {
                Chart1.Series[0].Points.AddXY(item.Key, item.Value);
            }

            // Add a title text 
            // (Use Titles properties of the chart to add one title) 
            Chart1.Titles[0].Text = "Issues by Status";
        }

        // Collect data from the issue list
        // Count the number of issues for each status. 
        // 
        protected Dictionary<string, int> CollectData(List<Issue> issue_list)
        {
            Dictionary<string, int> data = new Dictionary<string, int>();

            foreach (Issue issue in issue_list)
            {
                string status = issue.fields.Find(x => x.name.Equals("Status")).value;
                if (data.ContainsKey(status))
                {
                    data[status]++;
                }
                else
                {
                    data.Add(status, 1);
                }
            }

            return data;
        }

    }
}