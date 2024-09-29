<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Bind.aspx.cs" Inherits="HuakeWeb.Pages.Bind" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #bind {
            height: 100%;
            background: #f6f6f6;
        }

        .container {
        }

        .title {
            padding-top: 30%;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="bind">
        <h1 class="title">用户绑定</h1>
        <van-form class="container" v-on:submit="onSubmit">
            <van-field
                v-model="form.code"
                name="code"
                label="供应商编码"
                placeholder="请填写供应商编码"
                v-bind:rules="[{ required: true, message: '请填写供应商编码' }]">
            </van-field>
            <van-field
                v-model="form.name" 
                name="name"
                label="供应商名称"
                placeholder="请填写供应商名称"
                v-bind:rules="[{ required: true, message: '请填写供应商名称' }]">
            </van-field>
            <div style="margin: 16px;">
                <van-button style="border-radius: 10px" block type="info" native-type="submit">查询</van-button>
            </div>
        </van-form>
    </div>
    <script>
        new Vue({
            el: '#bind',
            data() {
                return {
                    form: {
                        code: '03000032',
                        name: '常州嘉奕运输有限公司',
                    }
                };
            },
            methods: {
                onSubmit(values) {
                    sendPost("handler.ashx?action=query-vendor", Object.assign({}, this.form)).then(res => { console.log(res) }).catch(e => { })
                },
            }
        });
    </script>
</asp:Content>
