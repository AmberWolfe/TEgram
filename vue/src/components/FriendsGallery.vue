<template>
  <div id="background">
    <h1 v-if="isNotLoading">{{ userObject.username }}'s Photo Gallery</h1>
    <div class="big-parent">
      <!-- load screen -->
        <div class="loading" v-if="isLoading">
            <img class="load-image" src="../images/matrix.gif" />
        </div>
      <div class="parent" v-for="obj in userObject.photos" :key="obj.photoId">
        <div id="grid-tile"> 
          <img class="gallery-img" @click="sharePhotoId(obj.photoId)" :src="obj.url" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import photoService from "@/services/PhotoService.js";
export default {
  name: "friends-gallery",
  data() {
    return {
      userObject: {},
      userId: 0,
      isLoading: true,
      isNotLoading: false
    };
  },

methods:{
  sharePhotoId(id) {
      this.$router.push({ name: "full-details", params: { data: id } });
    },
},

  created() {
    this.userId = this.$route.params.data;

    photoService.get(this.userId).then((response) => {
      this.userObject = response.data;
      this.isLoading = false;
      this.isNotLoading = true;
    });
  },
};
</script>

<style>
.gallery-img{
opacity: 1;
}
.gallery-img:hover{
  opacity: 0.6;
}




.big-parent{
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  justify-content: flex-start;
  margin-top: 50px;
  margin-left: 100px;
  margin-right: 100px;
}

#grid-tile {
  height: 200px;
  width: 200px;
  display: flex;
  border-radius: 1%;
  margin: 10px;
  background-color: white;
  box-shadow: 3px 3px rgba(0, 0, 0, 0.4);
}
img {
  height: 100%;
  width: auto;
  margin: auto;
  overflow: hidden;
}

.load-image{
  border-radius: 15px;
}
</style>