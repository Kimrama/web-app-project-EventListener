document
    .getElementById("register-form")
    .addEventListener("submit", function (event) {
        event.preventDefault();

        const birthDateInp = document.getElementById("birth-date");
        const errorMessage = document.getElementById("bdate-error-message");

        const today = new Date();
        const birthdate = new Date(birthDateInp.value);

        if (isNaN(birthdate.getTime()) || birthdate > today) {
            errorMessage.style.display = "inline";
            return;
        } else {
            errorMessage.style.display = "none";
        }

        const passwd = document.getElementById("passwd").value;
        const confirmPasswd = document.getElementById("conf-passwd").value;
        const confirmPasswdError = document.getElementById(
            "confpasswd-error-message"
        );
        if (passwd !== confirmPasswd) {
            confirmPasswdError.style.display = "inline";
        } else {
            confirmPasswdError.style.display = "none";
        }
    });

function changeColor() {
    console.log("change");
    let select = document.getElementById("gender");
    if (select.value === "male") {
        select.style.backgroundColor = "#ADD8E6";
    } else if (select.value === "female") {
        select.style.backgroundColor = "#FFC0CB";
    } else {
        select.style.backgroundColor = "white";
    }
}
