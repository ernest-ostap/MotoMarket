"use strict";

// 1. Pobieramy dane z ukrytych pól w HTML (zdefiniujemy je zaraz w widoku)
const apiUrl = document.getElementById("chatData").dataset.apiUrl;
const recipientId = document.getElementById("chatData").dataset.recipientId;
const listingId = document.getElementById("chatData").dataset.listingId || null;
const currentUserId = document.getElementById("chatData").dataset.currentUserId;

// 2. Pobieramy Token JWT z ciasteczka (funkcja pomocnicza)
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}
//const token = getCookie("JWT"); 
const token = document.getElementById("chatData").dataset.jwtToken;

// 3. Konfiguracja połączenia SignalR
// WAŻNE: Musimy przekazać token, żeby Hub wiedział kim jesteśmy!
const connection = new signalR.HubConnectionBuilder()
    .withUrl(apiUrl + "/chatHub", {
        accessTokenFactory: () => token
    })
    .build();

// 4. Obsługa przychodzącej wiadomości (To wywoła Backend: await Clients.User(...).SendAsync("ReceiveMessage", ...))
connection.on("ReceiveMessage", function (senderId, message, listingId) {
    addMessageToUI(senderId, message, new Date().toLocaleTimeString());
});

// 5. Start połączenia
connection.start().then(function () {
    console.log("SignalR Connected!");
    document.getElementById("sendButton").disabled = false;
    loadHistory(); // Po połączeniu pobierz historię
}).catch(function (err) {
    return console.error(err.toString());
});

// 6. Wysyłanie wiadomości (Kliknięcie guzika)
document.getElementById("sendButton").addEventListener("click", function (event) {
    const messageInput = document.getElementById("messageInput");
    const message = messageInput.value;

    if (!message) return;

    // Wywołanie metody na Serwerze (Hubie)
    connection.invoke("SendMessage", recipientId, message, listingId ? parseInt(listingId) : null)
        .then(() => {
            // Jak się uda wysłać, dodajemy dymek u siebie
            addMessageToUI(currentUserId, message, "Teraz");
            messageInput.value = "";
        })
        .catch(function (err) {
            return console.error(err.toString());
        });

    event.preventDefault();
});

// --- Funkcje pomocnicze ---

// Dodawanie dymka do HTML
function addMessageToUI(senderId, message, time) {
    const container = document.getElementById("messagesList");
    const isMine = senderId === currentUserId;

    const div = document.createElement("div");
    div.classList.add("d-flex", "mb-3", isMine ? "justify-content-end" : "justify-content-start");

    const bubble = document.createElement("div");
    bubble.classList.add("p-3", "rounded-3", "shadow-sm");

    // Style dymków (Niebieski dla mnie, szary dla rozmówcy)
    if (isMine) {
        bubble.classList.add("bg-primary", "text-white");
        bubble.style.maxWidth = "70%";
        bubble.style.borderBottomRightRadius = "0";
    } else {
        bubble.classList.add("bg-light", "text-dark", "border");
        bubble.style.maxWidth = "70%";
        bubble.style.borderBottomLeftRadius = "0";
    }

    bubble.innerHTML = `<div>${message}</div><small class="opacity-75" style="font-size: 0.7em;">${time}</small>`;

    div.appendChild(bubble);
    container.appendChild(div);

    // Scroll na dół
    container.scrollTop = container.scrollHeight;
}

// Pobieranie historii (zwykłe API, nie SignalR)
function loadHistory() {
    fetch(`${apiUrl}/api/Chat/history/${recipientId}`, {
        headers: { "Authorization": "Bearer " + token }
    })
        .then(response => response.json())
        .then(data => {
            const container = document.getElementById("messagesList");
            container.innerHTML = ""; // Wyczyść
            data.forEach(msg => {
                addMessageToUI(msg.senderId, msg.content, new Date(msg.sentAt).toLocaleTimeString());
            });
        });
}