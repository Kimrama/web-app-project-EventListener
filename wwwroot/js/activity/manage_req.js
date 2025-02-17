function updateParticipantStatus(userId, activityOwnerId, activityCreatedAt, status) {
    var xhr = new XMLHttpRequest();
    var url = "/Activity/UpdateParticipantStatus";

    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    console.log("userId:", userId);
    console.log("activityOwnerId:", activityOwnerId);
    console.log("activityCreatedAt:", activityCreatedAt); // ไม่ต้องแปลงเป็น DateTime
    console.log("status:", status);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                alert(response.message);
                location.reload();
            } else {
                alert("เกิดข้อผิดพลาด: " + xhr.responseText);
            }
        }
    };

    var data = "userId=" + encodeURIComponent(userId) +
               "&activityOwnerId=" + encodeURIComponent(activityOwnerId) +
               "&activityCreatedAt=" + encodeURIComponent(activityCreatedAt) + // ส่งแบบ string ตรงๆ
               "&status=" + encodeURIComponent(status);

    xhr.send(data);
}
