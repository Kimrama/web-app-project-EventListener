const dateFilterBtn = document.getElementById("date-filter");
const month = [
    "ม.ก.",
    "ก.พ.",
    "มี.ค.",
    "เม.ย",
    "พ.ค.",
    "มิ.ย.",
    "ก.ค.",
    "ส.ค.",
    "ก.ย.",
    "ต.ค.",
    "พ.ย.",
    "ธ.ค.",
];
let checkboxData = [];
function openPopupDate() {
    document.getElementById("popupOverlayDate").style.display = "flex";
}
function openPopupTag() {
    document.getElementById("popupOverlayTag").style.display = "flex";
    console.log("object");
}

function showFilterList() {
    const tagFilterList = document.getElementById("tag-filter-list");
    const tagFilter = document.getElementById("tag-filter");

    tagFilterList.innerHTML = "";

    const checkboxes = document.querySelectorAll('input[type="checkbox"]');

    checkboxes.forEach((checkbox) => {
        if (checkbox.checked) {
            const tag = document.createElement("div");
            tag.innerHTML = `<i class="fa-solid fa-xmark"></i> ${checkbox.name}`;
            tag.classList.add("tag");
            tag.addEventListener("click", () => {
                checkbox.checked = false;
                updateInputTag();
                applyFilter();
            });
            tagFilterList.appendChild(tag);
        }
    });
    if (tagFilterList.innerHTML === "") {
        tagFilter.innerHTML = "All";
    } else {
        tagFilter.innerHTML = "Select";
    }
    console.log(checkboxData);
}

function updateInputTag() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');

    checkboxData = Array.from(checkboxes).map((checkbox) => ({
        name: checkbox.name,
        checked: checkbox.checked,
    }));

    showFilterList();
}
function closePopupTag(event) {
    if (!event || event.target === document.getElementById("popupOverlayTag")) {
        document.getElementById("popupOverlayTag").style.display = "none";
    }
    updateInputTag();
    applyFilter();
}
function formatDate(dateStr) {
    let date = new Date(dateStr);
    let day = String(date.getDate()).padStart(2, "0");
    let month = month[date.getMonth()];
    let year = date.getFullYear();
    return `${day} ${month} ${year}`;
}
function closePopupDate(event) {
    if (
        !event ||
        event.target === document.getElementById("popupOverlayDate")
    ) {
        document.getElementById("popupOverlayDate").style.display = "none";
    }

    const startDateInp = document.getElementById("start-date");
    const endDateInp = document.getElementById("end-date");

    const startDate = new Date(startDateInp.value);
    const endDate = new Date(endDateInp.value);

    if (startDate == "Invalid Date" && endDate == "Invalid Date") {
        dateFilterBtn.textContent = "All";
        return;
    }

    if (startDate > endDate) {
        alert("INVALID DATE FILTER");
        startDateInp.value = "";
        endDateInp.value = "";
    } else if (startDate == "Invalid Date") {
        dateFilterBtn.textContent =
            "ไม่เกิน " +
            String(endDate.getUTCDate()) +
            " " +
            String(month[endDate.getUTCMonth()]) +
            " " +
            String(endDate.getUTCFullYear());
    } else if (endDate == "Invalid Date") {
        dateFilterBtn.textContent =
            "ตั้งแต่  " +
            String(startDate.getUTCDate()) +
            " " +
            String(month[startDate.getUTCMonth()]) +
            " " +
            String(startDate.getUTCFullYear());
    } else {
        dateFilterBtn.textContent =
            String(startDate.getUTCDate()) +
            " " +
            String(month[startDate.getUTCMonth()]) +
            " " +
            String(startDate.getUTCFullYear()) +
            " - " +
            String(endDate.getUTCDate()) +
            " " +
            String(month[endDate.getUTCMonth()]) +
            " " +
            String(endDate.getUTCFullYear());
    }
    applyFilter();
}
function getDaysToBeginWord(date) {
    const now = new Date();

    if (date.getUTCDate() == now.getUTCDate()) {
        return "กิจกรรมจะเกิดขึ้นวันนี้";
    } else {
        const timeDiff = date.getTime() - now.getTime();
        const dateDiff = Math.ceil(timeDiff / (1000 * 60 * 60 * 24));
        if (timeDiff < 0) {
            return "กิจกรรมเริ่มไปแล้ว";
        } else if (timeDiff == 1) {
            return "กิจกรรมจะเริ่มวันพรุ่งนี้";
        } else {
            return `กิจกรรมจะเริ่มในอีก ${dateDiff} วัน`;
        }
    }
}
function createUserIcon(userJoinActivity, userJoinActivityCount) {
    let iconList = userJoinActivity.map((uja) => {
        if (uja.user.userImageUrl == null) {
            return `<img src="/user.png" alt="participant-img" class="participant-img">`;
        } else {
            return `<img src="${uja.user.userImageUrl}" alt="participant-img" class="participant-img">`;
        }
    });

    iconList = iconList.slice(0, 5);

    if (userJoinActivityCount > 5) {
        iconList.push(`<span style="background-color: gray; padding: 14px; border-radius: 20px; font-size: 17px;"
        ><i class="fa-regular fa-user" style="margin-right: 8px;"></i>${
            userJoinActivityCount - 5
        } +</span
    >`);
    }
    return iconList.join("");
}

function getSelectedTags() {
    const checkboxes = document.querySelectorAll(".filter-checkbox");
    const selectedTags = new Set();
    checkboxes.forEach((checkbox) => {
        if (checkbox.checked) {
            selectedTags.add(checkbox.name);
        }
    });
    console.log(selectedTags);
    return Array.from(selectedTags);
}
async function applyFilter() {
    const startDateValue = document.getElementById("start-date").value;
    const endDateValue = document.getElementById("end-date").value;
    const searchWord = document.getElementById("search-word").value;
    const tags = getSelectedTags();
    const params = new URLSearchParams();
    if (startDateValue) params.append("startDate", startDateValue);
    if (endDateValue) params.append("endDate", endDateValue);
    if (tags.length > 0) {
        tags.forEach((tag) => params.append("tags", tag));
    }
    if (searchWord) params.append("searchWord", searchWord);
    console.log(params.toString());
    try {
        const response = await fetch(
            `/Home/FilterActivityData?${params.toString()}`
        );
        if (!response.ok) throw new Error("Failed to fetch filtered data");

        const data = await response.json();
        console.log("Filtered Data:", data);
        updateActivityCards(data);
    } catch (error) {
        console.error("Error fetching filtered activities:", error);
    }
}
function updateActivityCards(data) {
    const container = document.querySelector(".activity-card-container");
    container.innerHTML = "<p>กำลังโหลดกิจกรรม...</p>";

    setTimeout(() => {
        container.innerHTML = "";
        data.forEach((activity) => {
            const card = document.createElement("a");
            card.classList.add("activity-card");
            card.href = `activity/detail/${activity.activityIdEncode}`;

            let categoryClass = "";
            switch (activity.activityTagCategory) {
                case "Education":
                    categoryClass = "education";
                    break;
                case "Exercise & Sports":
                    categoryClass = "exercise-sports";
                    break;
                case "Arts & Culture":
                    categoryClass = "arts-culture";
                    break;
                case "Social":
                    categoryClass = "social";
                    break;
                default:
                    categoryClass = "";
            }
            const startDate = new Date(Date.parse(activity.startDate));
            let formattedDate = startDate.toLocaleDateString("th-TH", {
                weekday: "long",
                day: "numeric",
                month: "long",
                year: "numeric",
            });

            let formattedTime = activity.startTime.substring(0, 5) + " น.";

            card.innerHTML = `
                <div class="header-tag-card ${categoryClass}">${
                activity.activityTagCategory
            }</div>
                <div class="body-card">
                    <div class="tag-line">
                        <span class="tag">${activity.activityTagId}</span>
                    </div>
                    <div class="time-info">
                        <span>${formattedDate}, ${formattedTime}</span>
                        <span>${getDaysToBeginWord(startDate)}</span>
                    </div>
                    <div class="card-info">
                        <div class="info">
                            <article class="activity-name">${
                                activity.activityName
                            }</article>
                            <span class="location">${activity.location}</span>
                            <div class="user-avatar">
                                ${createUserIcon(
                                    activity.userJoinActivity,
                                    activity.userJoinActivityCount
                                )}
                            </div>
                            <span class="participant-info">${
                                activity.userJoinActivityCount
                            }/${activity.participantLimit} คนจะไป</span>
                        </div>
                        <img src="${
                            activity.activityImageUrl
                        }" alt="activity picture" class="activity-img">
                    </div>
                </div>
            `;
            container.appendChild(card);
        });
    }, 500);
}

document.getElementById("search-word").addEventListener("change", () => {
    applyFilter();
});

async function fetchActivityData() {
    try {
        const response = await fetch("/Home/fetchActivityData");
        if (!response.ok) throw new Error("Failed to fetch data");

        const data = await response.json();
        console.log("All Data:", data);
        updateActivityCards(data);
    } catch (error) {
        console.error("Error fetching activities:", error);
    }
}
fetchActivityData();
