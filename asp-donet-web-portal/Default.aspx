<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="findmydocs._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Contoso Document Tracking Portal</h1>
        <p class="lead">Welcome to contoso document tracking portal.</p>
    </div>
    <asp:Label ID="Label3" Visible="False" runat="server" Font-Bold="True" ForeColor="#0066FF" Text="Select a file below to see access activity by other users."></asp:Label> <br />
    <div class="row">

    </div>

        <div>
            <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Height="193px" Width="1048px" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                 <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="Button1" runat="server" CommandName="select" Text="Track File" />
                </ItemTemplate>
            </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
            <br />

        </div>
    <asp:Label ID="Label2" Visible="False" runat="server" Font-Bold="True" ForeColor="#0066FF"></asp:Label> <br />
    <asp:Label ID="Label1" Visible="False" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
            <div>
            <asp:GridView ID="GridView2" OnRowDataBound="GridView2_RowDataBound" runat="server" CellPadding="4" ForeColor="#333333" Height="193px" Width="1048px" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" EnableSortingAndPagingCallbacks="True">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerSettings Mode="NextPreviousFirstLast" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
            <br />

        </div>
</asp:Content>
