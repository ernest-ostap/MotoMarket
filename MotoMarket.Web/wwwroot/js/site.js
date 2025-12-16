// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    updateUnreadCount();
});

function updateUnreadCount() {
    // Sprawdź czy user jest zalogowany (czy ma ciastko JWT?)
    // Możesz też sprawdzić czy element badge istnieje
    const badge = document.getElementById("msg-badge");
    if (!badge) return;

    // Musisz mieć token w JS (podobnie jak w chat.js) LUB 
    // zrób w Web Controllerze akcję proxy, która ma ciastko i sama woła API.
    // Opcja PROXY (Web Controller) jest łatwiejsza dla Layoutu:

    fetch('/Chat/GetUnreadCount') // <-- Zaraz dodamy to do Web/ChatController
        .then(response => {
            if (response.ok) return response.json();
            throw new Error('Not logged in');
        })
        .then(count => {
            if (count > 0) {
                badge.innerText = count;
                badge.style.display = "inline-block";
            } else {
                badge.style.display = "none";
            }
        })
        .catch(err => { /* ignoruj błędy */ });
}