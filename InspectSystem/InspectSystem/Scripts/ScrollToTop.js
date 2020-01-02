$(document).ready(function () {

    // When the user scrolls down 20px from the top of the document, show the button
    window.onscroll = function () {
        scrollFunction();
    };
});

function scrollFunction() {
    var goToTopBtn = document.getElementById("goToTopBtn");
    var submitScrollBtn = document.getElementById("submitScrollBtn");
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        if (goToTopBtn !== null) {
            document.getElementById("goToTopBtn").style.display = "block";
        }
        if (submitScrollBtn !== null) {
            document.getElementById("submitScrollBtn").style.display = "block";
        }
    } else {
        if (goToTopBtn !== null) {
            document.getElementById("goToTopBtn").style.display = "none";
        }
        if (submitScrollBtn !== null) {
            document.getElementById("submitScrollBtn").style.display = "none";
        }
    }
    /* If scroll to bottom. */
    if (document.documentElement.scrollTop + window.innerHeight === document.documentElement.scrollHeight) {
        if (submitScrollBtn !== null) {
            document.getElementById("submitScrollBtn").style.display = "none";
        }
    } 
}

// When the user clicks on the button, scroll to the top of the document
function topFunction() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}