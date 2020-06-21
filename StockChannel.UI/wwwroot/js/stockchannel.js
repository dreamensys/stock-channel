"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/StockChannelHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("messageReceived",appendMessageToChat);
connection.on("messageListReceived",setMessagesToChat);

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", onSendButtonClicked);

/* EVENTS */
function onSendButtonClicked(event) {
    sendNewMessageToServer();
    var messageInput = document.getElementById("messageInput");
    messageInput.value = '';
    event.preventDefault();
}

/* FUNCTIONS */
function sendNewMessageToServer() {
    var user = document.getElementById("userInput").value;
    var newMessage = {
        sender: user,
        content: document.getElementById("messageInput").value
    };
    connection.invoke("SendMessage", newMessage).then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });
}
function appendMessageToChat(element) {
    const message = element.content;
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = `${element.sender}: ${msg}\n`;

    var chat = document.getElementById("messagesList");
    chat.value += encodedMsg;
    chat.scrollTop = 99999;
}
function setMessagesToChat(messages) {
    var chat = document.getElementById("messagesList");
    var text = '';
    messages.forEach(function(element) {
        text +=  `${element.sender}: ${element.content}\n`;
    })
    chat.value = text;
    chat.scrollTop = 99999;
}

