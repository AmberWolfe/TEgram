import Vue from 'vue'
import App from './App.vue'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import router from './router/index'
import store from './store/index'
import axios from 'axios'
import AWS from 'aws-sdk'
import crypto from 'crypto'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.config.productionTip = false

Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.use(AWS)
Vue.use(crypto)

axios.defaults.baseURL = process.env.VUE_APP_REMOTE_API;

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app')