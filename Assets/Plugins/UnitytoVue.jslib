mergeInto(LibraryManager.library, {

  NicknametoVue: function(str1, str2){
    getNickname(Pointer_stringify(str1), Pointer_stringify(str2));
  },

  HitNicknametoVue: function(str1, str2)
  {
    getHitNickname(Pointer_stringify(str1),Pointer_stringify(str2));
  },
  
  GoBaekHouse: function(){
    if (window.confirm("백준사이트로 이동하시겠습니까?")) {
      window.open("https://www.acmicpc.net/", "이동합니다");
    }
  },
  GoSWEAHouse: function(){
    if (window.confirm("SWEA사이트로 이동하시겠습니까?")) {
      window.open("https://swexpertacademy.com/main/main.do", "이동합니다");
    }
  },

  ShowBoard: function(){
    showBoard();
  },

  ShowTime: function(){
    showTime();
  },

  GetUserInfo: function(){
    var token = localStorage.getItem("accessToken");
    var buffer = _malloc(lengthBytesUTF8(token) + 1);
    writeStringToMemory(token, buffer);
    return buffer;
  },
  SaveRoompk: function(roomPk){
    getRoomPk(Pointer_stringify(roomPk));
  },
})