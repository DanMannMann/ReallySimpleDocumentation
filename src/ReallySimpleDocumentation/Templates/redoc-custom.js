document.addEventListener('DOMContentLoaded', (event) => {
    const targetNode = document.getRootNode();
    const config = { attributes: false, childList: true, subtree: true };

    // Callback function to execute when mutations are observed
    const callback = function (mutationsList, observer) {
        var link = document.querySelector("a[href$='/redoc/{{ApiShortName}}/swagger.json']");
        if (link) {
            link.setAttribute("href", "/swagger/{{ApiShortName}}/swagger.json");
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);

});