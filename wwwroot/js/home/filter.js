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
    console.log(
        startDate.getDate(),
        startDate.getMonth() + 1,
        startDate.getFullYear()
    );
}
