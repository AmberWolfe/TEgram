import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

/*
 * The authorization header is set for axios when you login but what happens when you come back or
 * the page is refreshed. When that happens you need to check for the token in local storage and if it
 * exists you should set the header so that it will be attached to each request
 */
const currentToken = localStorage.getItem('token')
const currentUser = JSON.parse(localStorage.getItem('user'));

if (currentToken != null) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${currentToken}`;
}

export default new Vuex.Store({
    state: {

        token: currentToken || '',
        user: currentUser || {



            // username: "",
            // userProfileUrl: "",
            // firstName: "",
            // lastName: "",

            // photos: [{
            //     url: "",
            //     userId: 0,
            //     comments: [],
            //     likes: []
            // }]


        },




    },
    mutations: {
        SET_AUTH_TOKEN(state, token) {
            state.token = token;
            localStorage.setItem('token', token);
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        },
        SET_USER(state, user) {
            //DELETE LINE 54 LATER
            user.photos[0].comments = ["hello", "world"];
            console.log(user);
            state.user = user;
            console.log(state.user)
            localStorage.setItem('user', JSON.stringify(user));

        },
        LOGOUT(state) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            state.token = '';
            state.user = {};
            axios.defaults.headers.common = {};
        },
        // SET_PIC(state, data) {
        //     state.user = data;
        // }

        SET_PHOTO_CARDS(state, data) {
            state.photoCards = data;
        },

        SET_PHOTO(state, data) {
            state.photo = data;
        }
    }
})