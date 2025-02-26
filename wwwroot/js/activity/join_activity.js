async function join_activity(activityIdHash) {
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
        `;

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