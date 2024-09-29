<%@ Page Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Auth.aspx.cs" Inherits="HuakeWeb.Pages.Auth" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <van-empty image="error" description="<%=HuakeWeb.Global.errMsg %>">
    </van-empty>
    <script></script>
</asp:Content>
