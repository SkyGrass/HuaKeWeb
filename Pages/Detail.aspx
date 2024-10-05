<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Master/Site1.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="HuakeWeb.Pages.Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>运费结算单详情</title>
    <style>
        #detail {
            width: 100vw;
            height: 100%;
            background: #f6f6f6;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="detail">
        <div style="height: calc(100vh - 138px)" v-if="list.length>0">
            <van-cell-group title="运费结算单">
                <van-cell v-for="item in list" v-bind:key="item.ID" v-bind:title-style="{flex:'3'}">
                    <template slot="title">
                        {{item.cCode}}
                        <van-tag type="default">{{item.cCardTypeCode}}</van-tag>
                    </template>
                    <template>
                        <div>￥{{item.iYFMoney_End}}</div>
                    </template>
                    <template slot="label">
                        <div>【日期】{{item.dDate |formatDate}}</div>
                    </template>
                </van-cell>
            </van-cell-group>
            <van-cell-group title="物流供应商运费确认">
                <van-cell title="预约入场时间" icon="edit" v-bind:value="form.dCardInDate  | formatDate" is-link
                    v-on:click="handleShowFile">
                </van-cell>
                <van-cell title="车号" icon="edit" v-bind:value="form.cCardNo" is-link
                    v-on:click="handleEdit('cCardNo','车号')">
                </van-cell>
                <van-cell title="司机" icon="edit" v-bind:value="form.cCardUser" is-link
                    v-on:click="handleEdit('cCardUser','司机')">
                </van-cell>
                <van-cell title="手机号" icon="edit" v-bind:value="form.cCardUserPhone" is-link
                    v-on:click="handleEdit('cCardUserPhone','手机号')">
                </van-cell>
                <van-cell title="快递单号" icon="edit" v-bind:value="form.cSendCode" is-link
                    v-on:click="handleEdit('cSendCode','快递单号')">
                </van-cell>
            </van-cell-group>
            <van-cell-group v-if="list.length>0" title="运费结算单详情">
                <van-cell v-for="item in list1" v-bind:key="item.ID" is-link v-bind:title-style="{flex:'3'}" v-on:click="handleChooseImg(item)">
                    <template slot="title">
                        <van-tag type="primary">{{item.iRowNo}}</van-tag>
                        {{item.cSourceCode}} 
                    </template>
                    <template>
                        {{item.iUploadNum}}/5
                    </template>
                    <template slot="label">
                        <div>【客户】{{item.cCusName}}</div>
                        <div>【来源】{{item.cSource}}</div>
                        <div>【客户地址】{{item.cAddress}}</div>
                        <div>【件数】{{item.iNum}}</div>
                    </template>
                </van-cell>
            </van-cell-group>
        </div>
        <van-empty style="height: calc(100vh - 140px)" v-else description="没有查询到记录"></van-empty>
        <van-popup v-model="showFilter" round position="bottom" v-bind:style="{ height: '50%' }">
            <van-datetime-picker
                v-model="currentDate"
                type="date"
                title="选择年月日"
                v-on:confirm="handleConfirmDate"
                v-on:cancel="showFilter = false"
                v-bind:min-date="minDate">
            </van-datetime-picker>
        </van-popup>
        <van-popup v-model="showEdit" v-bind:style="{ height: '200px' }" v-bind:close-on-click-overlay="false" closeable>
            <div style="width: 80vw">
                <h3 style="text-align: center">请填写</h3>
                <van-field size="large" label-width="70px" v-model="cur.keyword" v-bind:label="cur.label"
                    v-bind:placeholder="cur.placeholder" clearable>
                </van-field>
                <van-button style="width: 60vw; margin-top: 20px; margin-left: 10vw;" type="primary" size="normal"
                    v-on:click="handleSave" v-bind:loading="loading">
                    保存</van-button>
            </div>
        </van-popup>
        <van-popup v-model="showImg" v-on:closed="handleRecalc" v-bind:style="{ height: '500px' }" v-bind:close-on-click-overlay="false" closeable>
            <div style="width: 90vw">
                <h3 style="text-align: center">请选择照片(最多5张)</h3>
                <van-uploader v-model="fileList" multiple v-bind:before-read="handleBeforeRead" v-bind:max-size="3 * 1024 * 1024"
                    v-on:delete="handleDelete" v-on:oversize="handleOverSize" v-bind:max-count="5" style="width: calc(100% - 20px); padding: 10px">
                </van-uploader>
                <van-button style="width: 70vw; margin-top: 20px; margin-left: 10vw;" type="primary" size="normal" icon="upgrade"
                    v-on:click="handleUpload" v-bind:loading="loading">
                    上传图片(压缩后限3M)</van-button>
            </div>
        </van-popup>
    </div>
    <script>  
        const vaildState = [1, 2]
        new Vue({
            el: "#detail",
            filters: {
                formatDate(value) {
                    return new dayjs(value).format("YYYY-MM-DD")
                }
            },
            data() {
                return {
                    loading: false,
                    showFilter: false,
                    showEdit: false,
                    showImg: false,
                    finished: true,
                    list: [],
                    list1: [],
                    imageList: [],
                    fileList: [],
                    form: {
                        iState: 3,
                        dCardInDate: "",
                        cCardNo: "",
                        cCardUser: "",
                        cCardUserPhone: "",
                        cSendCode: "",
                    },
                    minDate: new Date(),
                    currentDate: new Date(),
                    cur: {
                        keyword: "",
                        label: "",
                        placeholder: ""
                    },
                    curRow: {},
                    querys: {},
                    initError: true
                }
            },
            methods: {
                handleConfirmDate(value) {
                    this.currentDate = value;
                    this.cur = { keyword: new dayjs(value).format("YYYY-MM-DD"), prop: 'dCardInDate' }
                    setTimeout(() => {
                        this.handleSave();
                    }, 300)
                },
                handleShowFile() {
                    if (vaildState.indexOf(this.form.iState) < 0) return this.$toast.fail("当前单据状态不可编辑");
                    this.showFilter = true;
                },
                handleEdit(prop, label) {
                    if (vaildState.indexOf(this.form.iState) < 0) return this.$toast.fail("当前单据状态不可编辑");
                    this.showEdit = true;
                    this.cur = Object.assign({}, this.cur, {
                        keyword: this.form[prop] || '',
                        prop: prop, label: label, placeholder: "请输入" + label
                    })
                },
                handleSave() {
                    if (this.cur.keyword == this.form[this.cur.prop]) return this.$toast.fail("没有修改");
                    this.loading = true; setTimeout(() => {
                        sendPost("handler.ashx?action=save-item", Object.assign({}, this.cur, this.querys)).then(res => {
                            this.loading = false;
                            const { state, msg } = res;
                            if (state == 'success') {
                                this.form[this.cur.prop] = this.cur.keyword;
                                this.showEdit = false;
                                this.showFilter = false;
                                return this.$toast.success('操作成功');
                            } else {
                                return this.$toast.fail(msg);
                            }
                        }).catch(e => {
                            this.loading = false;
                        }), 1000
                    });
                },
                handleChooseImg(item) {
                    this.curRow = Object.assign({}, item);
                    this.fileList = [];
                    sendPost("handler.ashx?action=query-img", { id: item.AutoID }).then(res => {
                        const { state, data } = res;
                        this.fileList = data.map(f => {
                            f.id = f.AutoID;
                            f.url = this.buildUrl(f.cPictureName);
                            return f;
                        });
                        this.showImg = true
                    }).catch(e => {
                        this.showImg = true
                    })
                },
                buildUrl(url) {
                    return window.location.origin + '/' + url.replace(/\\/g, '/');
                },
                handleRecalc() {
                    const len = this.fileList.filter(f => { return f.AutoID != void 0 }).length;
                    const { AutoID } = this.curRow
                    const index = this.list1.findIndex(f => { return f.AutoID == AutoID });
                    if (index > -1) {
                        let target = this.list1[index];
                        target.iUploadNum = len;
                        this.$set(this.list1, index, target)
                    }
                },
                handleBeforeRead(file) {
                    this.$toast.loading('读取文件')
                    if (!Array.isArray(file)) {
                        file = [file]
                    }
                    return new Promise(async (resolve, reject) => {
                        let newarr = []
                        for (const i in file) {
                            const res = await imageConversion.compressAccurately(file[i], 300)
                            const newfile = new File([res], file[i].name, { type: res.type, lastModified: Date.now() })
                            newarr[i] = newfile
                        }
                        resolve(newarr)
                        this.$toast.clear();
                    })
                },
                handleDelete(file) {
                    if (vaildState.indexOf(this.form.iState) < 0) return this.$toast.fail("当前单据状态不可编辑");
                    if (file.url) {
                        sendPost("handler.ashx?action=delete-img", { id: file.AutoID }).then(res => {
                            let { state, msg } = res;
                            if (state == 'success') {
                                return this.$toast.success(msg);
                            }
                            this.$toast.fail(msg)
                        }).catch(res => {
                        })
                    }
                },
                handleUpload(files) {
                    if (vaildState.indexOf(this.form.iState) < 0) return this.$toast.fail("当前单据状态不可编辑");
                    const len = this.fileList.filter(f => { return f.AutoID != void 0 }).length;
                    if (len == this.fileList.length) return this.$toast.fail("请先传入新照片");
                    this.fileList.filter(f => { return f.AutoID == void 0 }).map((file, i) => {
                        var formData = new FormData();
                        if (file.file) {
                            formData.append('rowId', this.curRow.AutoID);
                            formData.append("fileType", file.file.type);
                            formData.append("file", file.file);
                            const req = sendPost("handler.ashx?action=save-img", formData, 5 * 60 * 1000);
                            req.engine.upload.onprogress = (e) => {
                                const { lengthComputable, total, loaded } = e
                                const percent = (loaded / total) * 100;
                                file.status = percent >= 100 ? 'done' : 'uploading';
                                file.message = percent >= 100 ? '已完成' : (percent.toFixed(1) + '%');
                                this.$set(this.fileList, i + len, file);
                            }
                            req.then((res) => {
                                const { state, msg, data } = res;
                                if (state == 'success') {
                                    let target = this.fileList[i + len];
                                    target.status = 'done';
                                    target.message = '已完成';
                                    const { autoId, path } = data;
                                    target.url = this.buildUrl(path);
                                    target.id = autoId;
                                    target.AutoID = autoId;
                                    this.$set(this.fileList, i + len, target);
                                    return this.$toast.success(msg);
                                }
                                return this.$toast.fail(msg);
                            }).catch((err) => {
                                console.log(err)
                                file.status = 'fail';
                                file.message = '上传失败';
                                this.$set(this.fileList, i + len, file);
                            })
                        }
                    });
                },
                handleQuery(filter) {
                    sendPost("handler.ashx?action=query-record", filter).then(res => {
                        let { state, data } = res;
                        if (state == 'success') {
                            if (!Array.isArray(data)) {
                                data = [];
                            }
                            this.list = data.map(f => {
                                f.step1IsOk = f.cCardNo != '' && f.cCardUser != '' && f.cCardUserPhone != '';
                                f.step2IsOk = f.cSendCode != '';
                                return f;
                            });
                            if (this.list.length > 0) {
                                this.currentDate = dayjs(this.list[0].dCardInDate).toDate();
                                this.form = Object.assign({}, this.list[0])
                            }
                        }
                    })
                },
                handleOverSize(file) {
                    return this.$toast.fail('压缩后文件大小超过3M')
                },
                handleDetail(filter) {
                    sendPost("handler.ashx?action=query-detail", filter).then(res => {
                        let { state, data } = res;
                        if (state == 'success') {
                            if (!Array.isArray(data)) {
                                data = [];
                            }
                            this.list1 = data.map(f => { return f; });
                        }
                    })
                }
            },
            mounted() {
                this.querys = getQueryParams();
                this.handleQuery(this.querys);
                this.handleDetail(this.querys);
            }
        })
    </script>
</asp:Content>
