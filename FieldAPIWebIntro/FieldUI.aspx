<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FieldUI.aspx.cs" Inherits="FieldAPIWebIntro.FieldUI" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Field API Web Intro</title>
        <style type="text/css">
            #form1
        {
            height: 171px;
            width: 600px;
        }
        body
        {
            background-color:#c6d7e2; 
        }
        h1
        {
            color:#2a2e74; /*orange*/  
            text-align: center;
        }
        #iframeGlue
        {
            height: 442px;
            width: 561px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>My First Field API</h1>
    </div>
        <p>
        <asp:Label ID="LabelUserName" runat="server" Text="User Name" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxUserName" runat="server" Width="400px"></asp:TextBox>

        &nbsp;<asp:Button ID="ButtonLogin" runat="server" OnClick="ButtonLogin_Click" Text="Login" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelPassword" runat="server" Text="Password" Width="100px"></asp:Label>
            <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonLogout" runat="server" Enabled="False" OnClick="ButtonLogout_Click" Text="Logout" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelProject" runat="server" Text="Project" Width="100px"></asp:Label>
            <asp:DropDownList ID="DropDownListProjects" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListProjects_SelectedIndexChanged" Width="400px">
            </asp:DropDownList>
&nbsp;<asp:Button ID="ButtonProject" runat="server" OnClick="ButtonProject_Click" Text="Project" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelIssue" runat="server" Text="Issue" Width="100px"></asp:Label>
            <asp:DropDownList ID="DropDownListIssues" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListIssues_SelectedIndexChanged" Width="400px">
            </asp:DropDownList>
&nbsp;<asp:Button ID="ButtonIssue" runat="server" OnClick="ButtonIssue_Click" Text="Issue" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelNewIssue" runat="server" Text="New issue" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxNewIssue" runat="server" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonCreate" runat="server" OnClick="ButtonCreate_Click" Text="Create" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelRequest" runat="server" Text="Request"></asp:Label>
            <asp:TextBox ID="TextBoxRequest" runat="server" Height="60px" ReadOnly="True" TextMode="MultiLine" Width="580px"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="LabelResponse" runat="server" Text="Response"></asp:Label>
            &nbsp; 
            <asp:Label ID="LabelStatus" runat="server"></asp:Label>
            <asp:TextBox ID="TextBoxResponse" runat="server" Height="60px" TextMode="MultiLine" Width="580px"></asp:TextBox>
        </p>
        <asp:Button ID="ButtonReport" runat="server" OnClick="ButtonReport_Click" Text="Report" />
        <br />
        <asp:Chart ID="Chart1" runat="server" BackColor="WhiteSmoke" Palette="None" Width="580px">
            <Series>
                <asp:Series ChartType="Bar" Name="Series1" Palette="Pastel">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Name="Title1">
                </asp:Title>
            </Titles>
        </asp:Chart>
        <br />
    </form>
</body>
</html>
