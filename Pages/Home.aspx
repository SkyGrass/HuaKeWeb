<%@ Page Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="HuakeWeb.Pages.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            <van-cell title="开始日期" v-bind:value="currentDate1|formatDate" is-link v-on:click="showFilter1 = true"></van-cell>
            <van-cell title="结束日期" v-bind:value="currentDate2|formatDate" is-link v-on:click="showFilter2 = true"></van-cell>
            <div style="height: calc(100vh - 138px)">
                <van-cell-group v-if="list.length>0" v-bind:title="'运费结算单列表'+'('+list.length+')'">
                    <van-cell v-on:click="showAction = true" v-for="item in list" v-bind:key="item.ID" is-link v-bind:title-style="{flex:'3'}">
                        <template slot="title">
                            {{item.cCode}}
                         <van-tag type="primary">{{item.cCardTypeCode}}</van-tag>
                        </template>
                        <template>
                            <van-tag type="warning">{{item.step1IsOk?'已填写':'未填写'}}</van-tag>
                            <van-tag type="danger">{{item.step2IsOk?'已上传':'未上传'}}</van-tag>
                        </template>
                        <template slot="label">
                            <div>【日期】{{item.dDate |formatDate}}【运费】￥{{item.iYFMoney_End}}</div>
                        </template>
                    </van-cell>
                </van-cell-group>
            </div>
            <van-empty style="height: calc(100vh - 140px)" v-else description="没有查询到记录"></van-empty>
        </div>
        <div v-if="active ==1">
            <van-cell-group>
                <van-cell title="单元格" value="内容"></van-cell>
            </van-cell-group>
        </div>
        <van-tabbar v-model="active">
            <van-tabbar-item icon="home-o">主页</van-tabbar-item>
            <van-tabbar-item icon="friends-o">个人中心</van-tabbar-item>
        </van-tabbar>
        <van-action-sheet v-model="showAction"
            cancel-text="取消" title="请选择" close-on-click-action v-on:cancel="showAction = false"
            v-bind:actions="actions" v-on:select="onSelect">
        </van-action-sheet>
        <van-popup v-model="showFilter1" round position="bottom" v-bind:style="{ height: '50%' }">
            <van-datetime-picker
                v-model="currentDate1"
                type="date"
                title="选择年月日"
                v-on:confirm="handleConfirmDate1"
                v-on:cancel="showFilter1 = false"
                v-bind:min-date="minDate"
                v-bind:max-date="maxDate">
            </van-datetime-picker>
        </van-popup>
        <van-popup v-model="showFilter2" round position="bottom" v-bind:style="{ height: '50%' }">
            <van-datetime-picker
                v-model="currentDate2"
                type="date"
                title="选择年月日"
                v-on:confirm="handleConfirmDate2"
                v-on:cancel="showFilter2 = false"
                v-bind:min-date="minDate"
                v-bind:max-date="maxDate">
            </van-datetime-picker>
        </van-popup>
    </div>
    <script>
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
                    showAction: false,
                    showFilter1: false,
                    showFilter2: false,
                    active: 0,
                    finished: true,
                    list: [],
                    minDate: new Date(2020, 0, 1),
                    maxDate: new Date(2025, 10, 1),
                    currentDate1: new Date(2024, 06, 20),
                    currentDate2: new Date(),
                    actions: [
                        { id: 0, name: '去填写', subname: "入场时间、车号、司机、手机号" },
                        { id: 1, name: '查详情', subname: "上传照片、快递单号" }]
                }
            },
            methods: {
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
                onSelect(item) { },
                handleQuery() {
                    sendPost("handler.ashx?action=query-record",
                        {
                            startDate: new dayjs(this.currentDate1).format("YYYY-MM-DD"),
                            endDate: new dayjs(this.currentDate2).format("YYYY-MM-DD")
                        }).then(res => {
                            if (!Array.isArray(res)) {
                                res = [];
                            }
                            this.list = res.map(f => {
                                f.step1IsOk = f.cCardNo != '' && f.cCardUser != '' && f.cCardUserPhone != '';
                                f.step2IsOk = f.cSendCode != '';
                                return f;
                            });
                        })
                }
            },
            mounted() {
                return this.handleQuery();
            }
        })
    </script>
</asp:Content>




