window.onload = function () {
    setTimeout(function () {
        const el = document.querySelector("div.text-center h1");
        if (el !== null) {
            el.innerHTML = "Web Catalog";
        }
    }, 600);
}