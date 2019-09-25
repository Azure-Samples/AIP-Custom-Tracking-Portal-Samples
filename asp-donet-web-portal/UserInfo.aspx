<%@ Page Title="User Info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true" CodeBehind="UserInfo.aspx.cs" Inherits="findmydocs.UserInfo" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <asp:Panel ID="GetToken" runat="server" Visible="false">
        <p>The token for accessing the Graph API has expired. Click <asp:LinkButton runat="server" OnClick="Unnamed_Click">here</asp:LinkButton> to sign-in and get a new access token.</p>
    </asp:Panel>
    <asp:Panel ID="ShowData" runat="server">
          <asp:FormView ID="UserData" runat="server" 
        ItemType="Microsoft.Azure.ActiveDirectory.GraphClient.IUser" 
        RenderOuterTable="false" DefaultMode="ReadOnly"
        ViewStateMode="Disabled">
        <ItemTemplate>
            <table class="table table-bordered table-striped">
                <tr>
                    <td>Display Name</td>
                    <td><%#: Item.DisplayName %></td>
                </tr>
                <tr>
                    <td>First Name</td>
                    <td><%#: Item.GivenName %></td>
                </tr>
                <tr>
                    <td>Last Name</td>
                    <td><%#: Item.Surname %></td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:FormView>
    </asp:Panel>
  
</asp:Content>
