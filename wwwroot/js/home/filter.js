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
function openPopupDate() {
    document.getElementById("popupOverlayDate").style.display = "flex";
}
function openPopupTag() {
    document.getElementById("popupOverlayTag").style.display = "flex";
    console.log("object");
}

function closePopupTag(event) {
    if (!event || event.target === document.getElementById("popupOverlayTag")) {
        document.getElementById("popupOverlayTag").style.display = "none";
    }
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
}
