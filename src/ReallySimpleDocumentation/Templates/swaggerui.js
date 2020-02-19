document.addEventListener('DOMContentLoaded', (event) => {
    const targetNode = document;
    const config = { attributes: true, childList: true, subtree: true };

    // Ensure favicon has stuck
    var icons = Array.prototype.slice.call(document.querySelectorAll("link[rel*='icon']"));
    for (var i of icons) {
        if (i.href !== "{{FavIconUrl}}") {
            i.href = "{{FavIconUrl}}";
        }
    }

    // Callback function to execute when mutations are observed
    const callback = function (mutationsList, observer) {
        // Ensure favicon has stuck
        var icons = Array.prototype.slice.call(document.querySelectorAll("link[rel*='icon']"));
        for (var i of icons) {
            if (i.href !== "{{FavIconUrl}}") {
                i.href = "{{FavIconUrl}}";
            }
        }

        var link = document.querySelector("a.link[href='/swaggerui/{{ApiShortName}}/swagger.json']");
        if (link) {
            link.setAttribute("href", "/swagger/{{ApiShortName}}/swagger.json");
            link.innerHTML = "<span>/swagger/{{ApiShortName}}/swagger.json</span>";
        }

        // Add anchors with redoc-compatible id's for operations
        var doneDivs = Array.prototype.slice.call(document.querySelectorAll("span[id^='operation/']+div[id^='operations-']"));
        var divs = document.querySelectorAll("div[id^='operations-']");
        var regex = /operations-(.*)-(.*)\\[\.]([^\.]*)/;
        for (var d of divs) {
            if (doneDivs.some(x => x.id === d.id)) continue;
            var result = regex.exec(d.id);
            if (result) {
                var a = document.createElement('span');
                a.id = "operation/" + result[2] + "." + result[3];
                d.parentElement.insertBefore(a, d);
            }
        }

        // Add anchors with redoc-compatible id's for tags
        var doneHs = Array.prototype.slice.call(document.querySelectorAll("span[id^='tag/']+h4[id^='operations-tag-']"));
        var hs = document.querySelectorAll("h4[id^='operations-tag-']");
        var regexH = /operations-tag-([^\.]*)/;
        for (var h of hs) {
            if (doneHs.some(x => x.id === h.id)) continue;
            var resultH = regexH.exec(h.id);
            if (resultH) {
                var s = document.createElement('span');
                s.id = "tag/" + resultH[1];
                h.parentElement.insertBefore(s, h);
            }
        }

        // Ensure logo alt text is set
        var logoImg = document.querySelector('.topbar .wrapper .topbar-wrapper a.link img');
        if (logoImg && logoImg.getAttribute("alt") !== "{{LogoAltText}}") {
            logoImg.setAttribute("alt", "{{LogoAltText}}");
        }
        if (logoImg && logoImg.getAttribute("src") !== "{{LogoUrl}}") {
            logoImg.setAttribute("src", "{{LogoUrl}}");
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);

});