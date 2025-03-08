async function join_activity(activityIdHash,ownerId,createDate,userId,curStatus) {
    console.log(activityIdHash, ownerId, createDate, userId)
    try {
        const response = await fetch("/Activity/JoinActivity", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ activityIdHash })
        });

        if (response.status == 401) {
            window.location.href = "/account/login";
            return;
        }

        if(response.status == 400){
            const err = await response.json();
            console.log(err);
            alert(err.message);
            return;
        }

        if (!response.ok) {
            const err = await response.json();
            console.log(err);
            return;
        }

        const buttonGroupForOwnerOrUserDiv = document.querySelector(".btn-group-for-owner-or-user");
        buttonGroupForOwnerOrUserDiv.innerHTML = `
             <button class="wait-btn">
                <i class="fa-solid fa-hourglass-start fa-lg"></i>
                <span>รอการอนุมัติ</span>
            </button>
             <button class="cancel-wait-btn" >
                <i class="fa-solid fa-hourglass-start fa-lg"></i>
                <span>ยกเลิกคำขอ</span>
            </button>
        `;
        button = document.querySelector(".cancel-wait-btn")
        button.setAttribute("data-ownerid", ownerId);
        button.setAttribute("data-createdate", createDate);
        button.setAttribute("data-userid", userId);
        if(curStatus == "exit"){button.setAttribute("data-curstatus", "wait");}
        else if(curStatus == "exit2"){button.setAttribute("data-curstatus", "wait2");}
        else if(curStatus == "exit3"){button.setAttribute("data-curstatus", "wait3");}
        else if(curStatus == null){button.setAttribute("data-curstatus","wait")}

        document.querySelector(".cancel-wait-btn").addEventListener("click", function () {
            const ownerId = this.dataset.ownerid;
            const createDate = this.dataset.createdate;
            const joinUser = this.dataset.userid;
            const curStatus = this.dataset.curstatus;
            console.log("call1")
            updateStatus(ownerId, createDate, joinUser, "exit", curStatus);
            console.log("call2")
        });

        // document.getElementById("userJoinActivityCount-wait").textContent = `${data.userJoinActivityCount} ผู้เข้าร่วม`

        // const allParticipant = document.getElementById("all-participant");
        // allParticipant.innerHTML = '';
        // data.usersJoinActivity.forEach(u => {
        //     const participantDiv = document.createElement("div");
        //     participantDiv.classList.add("participant");

        //     const userImg = document.createElement("img");
        //     if(u.userImageUrl != null){
        //         userImg.src = u.userImageUrl;
        //     }
        //     else{
        //         userImg.src = "/user.png";
        //     }
        //     participantDiv.appendChild(userImg);

        //     const username = document.createElement("h4");
        //     username.textContent = u.userName;
        //     participantDiv.appendChild(username);

        //     allParticipant.appendChild(participantDiv);
    }
    catch (error) {
        console.log(error);
    }
}