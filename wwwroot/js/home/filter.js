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
function openPopup() {
    document.getElementById("popupOverlay").style.display = "flex";
}

function closePopup(event) {
    if (!event || event.target === document.getElementById("popupOverlay")) {
        document.getElementById("popupOverlay").style.display = "none";
    }

    const startDateInp = document.getElementById("start-date");
    const endDateInp = document.getElementById("end-date");

    const startDate = new Date(startDateInp.value);
    const endDate = new Date(endDateInp.value);

    dateFilterBtn.textContent =
        String(startDate.getUTCDate()) +
        String(month[startDate.getUTCMonth()]) +
        String(startDate.getUTCFullYear());
}
