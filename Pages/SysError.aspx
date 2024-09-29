<%@ Page Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="SysError.aspx.cs" Inherits="HuakeWeb.Pages.SysError" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p><%= HuakeWeb.Global.errMsg %></p>
</asp:Content>
