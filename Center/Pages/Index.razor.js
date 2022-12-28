export function focusElement(elId) {
    var el = document.getElementById(elId);
    var isFocused = (document.activeElement === el);
    if (!isFocused)
        el.focus();
}

export  function scrollElementIntoView(elId) {
    var el = document.getElementById(elId);
    el.scrollIntoView();
}