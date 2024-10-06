<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Bind.aspx.cs" Inherits="HuakeWeb.Pages.Bind" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>用户绑定</title>
    <style>
        #bind {
            width: 100vw;
            height: 100%;
            background: #f6f6f6;
        }

        .container {
        }

        .title {
            padding: 30% 0px 30px 0px;
            text-align: center;
            font-size: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="bind">
        <div v-if="isBind == 0">
            <div class="title">用户绑定</div>
            <van-form class="container" v-on:submit="onSubmit">
                <van-field
                    v-model="form.code"
                    name="code"
                    left-icon="friends-o"
                    label="供应商编码"
                    placeholder="请填写供应商编码" clearable
                    v-bind:rules="[{ required: true, message: '请填写供应商编码' }]">
                </van-field>
                <van-field
                    v-model="form.name"
                    name="name"
                    left-icon="label-o"
                    label="供应商名称"
                    placeholder="请填写供应商名称" clearable
                    v-bind:rules="[{ required: true, message: '请填写供应商名称' }]">
                </van-field>
                <van-field
                    v-model="form.phone"
                    name="phone"
                    left-icon="phone-o"
                    label="联系电话"
                    placeholder="请填写供应商联系电话" clearable
                    v-bind:rules="[{ required: true, message: '请填写供应商联系电话' }]">
                </van-field>
                <div style="margin: 16px;">
                    <van-button style="border-radius: 10px" block type="info" native-type="submit">查询</van-button>
                </div>
            </van-form>
        </div>
        <van-empty style="height: calc(100vh - 178px)" v-else description="已绑定,正在登录"></van-empty>
    </div>
    <script> 
        new Vue({
            el: '#bind',
            data() {
                return {
                    isBind:"<%=IsBind%>",
                    form: {
                        code: '',
                        name: '',
                        phone: '',
                    },
                    querys: {},
                };
            },
            methods: {
                onSubmit(values) {
                    sendPost("handler.ashx?action=query-vendor", Object.assign({}, this.form)).then(res => {
                        const { state, msg } = res
                        if (state == 'success') {
                            sendPost("handler.ashx?action=bind", Object.assign({}, this.form, this.querys))
                                .then(res1 => {
                                    const { state, msg } = res1
                                    if (state == 'success') {
                                        this.$toast.success(msg);
                                        setTimeout(() => {
                                            location.reload();
                                        }, 2000)
                                    } return this.$toast.fail(msg);
                                }).catch(e => {
                                })
                        }
                        return this.$toast.fail(msg);
                    }).catch(e => { })
                },
            },
            mounted() {
                this.querys = getQueryParams();
            }
        });
    </script>
</asp:Content>
