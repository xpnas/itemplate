(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-25a92dfb"],{"907b":function(e,t,r){"use strict";r.r(t);var n=function(){var e=this,t=e.$createElement,r=e._self._c||t;return r("div",{staticClass:"app-container"},[r("el-card",{scopedSlots:e._u([{key:"header",fn:function(){return[r("div",{staticClass:"card-header"},[r("span",[e._v("JWT验证")])])]},proxy:!0}])},[r("el-form",{ref:"settingForm",attrs:{model:e.jwt,"label-width":"20%","label-position":"right",rules:e.jwtRules}},[r("el-form-item",{attrs:{prop:"clockSkew",label:"ClockSkew"}},[r("el-input",{attrs:{placeholder:"ClockSkew"},model:{value:e.jwt.clockSkew,callback:function(t){e.$set(e.jwt,"clockSkew",t)},expression:"jwt.clockSkew"}},[e._v(">")])],1),r("el-form-item",{attrs:{prop:"audience",label:"Audience"}},[r("el-input",{attrs:{placeholder:"Audience"},model:{value:e.jwt.audience,callback:function(t){e.$set(e.jwt,"audience",t)},expression:"jwt.audience"}})],1),r("el-form-item",{attrs:{prop:"issuer",label:"Issuer"}},[r("el-input",{attrs:{placeholder:"Issuer"},model:{value:e.jwt.issuer,callback:function(t){e.$set(e.jwt,"issuer",t)},expression:"jwt.issuer"}})],1),r("el-form-item",{attrs:{prop:"issuerSigningKey",label:"IssuerSigningKey"}},[r("el-input",{attrs:{type:"textarea",placeholder:"IssuerSigningKey"},model:{value:e.jwt.issuerSigningKey,callback:function(t){e.$set(e.jwt,"issuerSigningKey",t)},expression:"jwt.issuerSigningKey"}})],1),r("el-form-item",{attrs:{prop:"expiration",label:"Expiration"}},[r("el-input",{attrs:{placeholder:"Expiration"},model:{value:e.jwt.expiration,callback:function(t){e.$set(e.jwt,"expiration",t)},expression:"jwt.expiration"}})],1),r("el-form-item",[r("el-button",{attrs:{type:"primary"},on:{click:function(t){return e.submitForm("settingForm")}}},[e._v("确认修改")])],1)],1)],1)],1)},i=[],s=r("f318"),a={data:function(){return{jwtRules:{clockSkew:[{required:!0,message:"请输入clockSkew",trigger:"blur"}],audience:[{required:!0,message:"请输入audience",trigger:"blur"}],issuer:[{required:!0,message:"请输入issuer",trigger:"blur"}],issuerSigningKey:[{required:!0,message:"请输入issuerSigningKey",trigger:"blur"}],expiration:[{required:!0,message:"请输入expiration",trigger:"blur"}]},jwt:{}}},created:function(){this.fetchData()},methods:{fetchData:function(){var e=this;Object(s["d"])().then((function(t){e.jwt=t.data}))},submitForm:function(e){var t=this;this.$refs[e].validate((function(e){t.$confirm("您修改了JWT授权，这将影响所有已登录用户导致需要重新登陆，确认修改吗","警告",{distinguishCancelAndClose:!0,confirmButtonText:"确定修改",cancelButtonText:"放弃修改",type:"warning",center:!0,callback:function(e){"confirm"==e&&Object(s["g"])(t.jwt).then((function(e){200==e.code?t.$message({message:"设置成功",type:"success"}):t.$message.error("设置失败")}))}})}))}}},u=a,o=r("2877"),c=Object(o["a"])(u,n,i,!1,null,null,null);t["default"]=c.exports},f318:function(e,t,r){"use strict";r.d(t,"c",(function(){return i})),r.d(t,"f",(function(){return s})),r.d(t,"d",(function(){return a})),r.d(t,"g",(function(){return u})),r.d(t,"e",(function(){return o})),r.d(t,"a",(function(){return c})),r.d(t,"b",(function(){return l}));var n=r("b775");function i(){return Object(n["a"])({url:"/settingsys/GetGlobal",method:"get"})}function s(e){return Object(n["a"])({url:"/settingsys/setGlobal",method:"post",params:e})}function a(){return Object(n["a"])({url:"/settingsys/getJWT",method:"get"})}function u(e){return Object(n["a"])({url:"/settingsys/setJWT",method:"post",data:e})}function o(e,t,r){return Object(n["a"])({url:"/settingsys/getUsers",method:"get",params:{query:e,page:t,pagesize:r}})}function c(e,t){return Object(n["a"])({url:"/settingsys/activeUser",method:"post",params:{userName:e,active:t}})}function l(e){return Object(n["a"])({url:"/settingsys/deleteUser",method:"post",params:{userName:e}})}}}]);