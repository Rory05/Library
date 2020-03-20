// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var number = document.getElementById("number");

number.addEventListener("input", function (event) {
    if (numbervalidity.typeMismatch) {
        number.setCustomValidity("I expect an e-mail, darling!");
    } else {
        email.setCustomValidity("");
    }
});

