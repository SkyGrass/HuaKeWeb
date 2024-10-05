<%@ Page Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="HuakeWeb.Pages.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>主页</title>
    <style>
        #home {
            width: 100vw;
            height: 100%;
            background: #f6f6f6;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="home">
        <div v-if="active ==0 ">
            <van-cell-group title="查询">
                <van-cell title="开始日期" icon="clock-o" v-bind:value="currentDate1|formatDate" is-link v-on:click="showFilter1 = true"></van-cell>
                <van-cell title="结束日期" icon="clock-o" v-bind:value="currentDate2|formatDate" is-link v-on:click="showFilter2 = true"></van-cell>
            </van-cell-group>
            <div style="height: calc(100vh - 178px)">
                <van-cell-group v-if="list.length>0" v-bind:title="'运费结算单列表'+'('+list.length+')'">
                    <van-cell v-on:click="handle2Detail(item)" v-for="item in list" v-bind:key="item.ID" is-link v-bind:title-style="{flex:'3'}">
                        <template slot="title">
                            {{item.cCode}}
                         <van-tag type="default">{{item.cCardTypeCode}}</van-tag>
                        </template>
                        <template>
                            <van-tag v-bind:type="item.step1IsOk?'primary':'warning'">{{item.step1IsOk?'已填写':'未填写'}}</van-tag>
                            <van-tag v-bind:type="item.step1IsOk?'primary':'warning'">{{item.step2IsOk?'已上传':'未上传'}}</van-tag>
                        </template>
                        <template slot="label">
                            <div>【日期】{{item.dDate |formatDate}}</div>
                            <div>【运费】￥{{item.iYFMoney_End}}</div>
                        </template>
                    </van-cell>
                </van-cell-group>
                <van-empty style="height: calc(100vh - 178px)" v-else description="没有查询到记录"></van-empty>
            </div>
        </div>
        <div v-if="active ==1">
            <van-notice-bar
                left-icon="volume-o" text="未能获取到您的微信身份，请尝试重新打开或者再次发起绑定" v-if="noBind">
            </van-notice-bar>
            <van-cell-group title="基本信息">
                <van-cell title="运输公司" icon="friends-o" v-bind:value="userInfo.UserName" v-bind:title-style="{flex:0.5}"></van-cell>
            </van-cell-group>
            <van-cell-group title="操作">
                <van-cell title="绑定微信" icon="delete-o" is-link v-on:click="handleBind" v-if="noBind"></van-cell>
                <van-cell title="解绑微信" icon="delete-o" is-link v-on:click="handleUnBind" v-else></van-cell>
            </van-cell-group>
        </div>
        <van-tabbar v-model="active" v-on:change="handleChange" v-bind:safe-area-inset-bottom="true">
            <van-tabbar-item icon="home-o">主页</van-tabbar-item>
            <van-tabbar-item icon="friends-o">个人中心</van-tabbar-item>
        </van-tabbar>
        <van-popup v-model="showFilter1" round position="bottom" v-bind:style="{ height: '50%' }">
            <van-datetime-picker
                v-model="currentDate1"
                type="date"
                title="选择年月日"
                v-on:confirm="handleConfirmDate1"
                v-on:cancel="showFilter1 = false"
                v-bind:min-date="minDate">
            </van-datetime-picker>
        </van-popup>
        <van-popup v-model="showFilter2" round position="bottom" v-bind:style="{ height: '50%' }">
            <van-datetime-picker
                v-model="currentDate2"
                type="date"
                title="选择年月日"
                v-on:confirm="handleConfirmDate2"
                v-on:cancel="showFilter2 = false"
                v-bind:min-date="minDate">
            </van-datetime-picker>
        </van-popup>
    </div>
    <script>
        const titles = ['主页', '个人中心'];
        new Vue({
            el: "#home",
            filters: {
                formatDate(value) {
                    return new dayjs(value).format("YYYY-MM-DD")
                }
            },
            data() {
                return {
                    loading: false,
                    showFilter1: false,
                    showFilter2: false,
                    active: 0,
                    finished: true,
                    list: [],
                    minDate: new Date(2020, 0, 1),
                    currentDate1: new Date(2024, 6, 20),
                    currentDate2: new Date(),
                    userInfo: { UserName: "" },
                    noBind: true,
                }
            },
            methods: {
                handleChange(index) {
                    document.title = titles[index]
                    if (index == 1) {
                        this.handleUserInfo();
                    }
                },
                handleConfirmDate1(value) {
                    this.currentDate1 = value;
                    this.showFilter1 = false;
                    this.handleQuery();
                },
                handleConfirmDate2(value) {
                    this.currentDate2 = value;
                    this.showFilter2 = false;
                    this.handleQuery();
                },
                handle2Detail(item) {
                    const { ID } = item;
                    window.location.href = 'detail?id=' + ID
                },
                handleUserInfo() {
                    sendPost("handler.ashx?action=user-info").then(res => {
                        const { state, msg, data } = res; 
                        if (state == 'success') {
                            this.userInfo = Object.assign({}, data[0])
                            this.noBind = false;
                        } else { this.userInfo.UserName = "未获取" }
                    }).catch(e => { })
                },
                handleQuery() {
                    this.list = []
                    sendPost("handler.ashx?action=query-record",
                        {
                            startDate: new dayjs(this.currentDate1).format("YYYY-MM-DD"),
                            endDate: new dayjs(this.currentDate2).format("YYYY-MM-DD")
                        }).then(res => {
                            let { data } = res;
                            if (!Array.isArray(data)) {
                                data = [];
                            }
                            this.list = data.map(f => {
                                f.step1IsOk = f.cCardNo != '' && f.cCardUser != '' && f.cCardUserPhone != '';
                                f.step2IsOk = f.cSendCode != '';
                                return f;
                            });
                        })
                },
                handleBind() {
                    location.href = "/pages/auth"
                },
                handleUnBind() {
                    this.$dialog.confirm({
                        title: '提示',
                        message: '您确定要解绑当前微信吗？',
                    }).then(res => {
                        sendPost("handler.ashx?action=unbind", {}).then(res => {
                            const { state, msg } = res
                            if (state == 'success') {
                                this.$toast.success(msg)
                                location.reload();
                            }
                            return this.$toast.fail(msg)
                        }).catch(e => { })
                    }).catch(e => { })
                }
            },
            mounted() {
                return this.handleQuery();
            }
        })
    </script>
</asp:Content>




