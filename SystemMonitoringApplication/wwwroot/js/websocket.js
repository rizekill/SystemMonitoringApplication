
var wsUri = "ws://localhost:63572/ws";
socket = new WebSocket(wsUri);

    socket.onopen = function(e) {
        console.log("Connection established");
        subscribe("HighLoadEvent");
        subscribe("ProcessObserveCompleteEvent");
        subscribe("ProcessObserveOpenEvent");
        subscribe("ProcessStateUpdateEvent");
    };

    socket.onclose = function (e) {
        console.log("Connection closed");
    };

    socket.onmessage = (event) => {
        const message = JSON.parse(event.data);

        switch (message.messageType) {
            case 'HighLoadEvent':
                onHighLoad(JSON.parse(message.data));
                break;
            case 'ProcessObserveCompleteEvent':
                removeProcess(JSON.parse(message.data).processState);
                break;
            case 'ProcessObserveOpenEvent':
                insertProcessState(JSON.parse(message.data).processState);
                break;
            case 'ProcessStateUpdateEvent':
                updateProcessState(JSON.parse(message.data).processState);
                break;
        }
    };

    function onHighLoad(data) {
        viewModal(data);
    }

    function subscribe(messageType) {
       sendWebSocketMessage("SubscribeCommand", {MessageType: messageType});
    }

    function unsubscribe(messageType) {
        sendWebSocketMessage("UnsubscribeCommand", { MessageType: messageType });
    }

    function sendWebSocketMessage(messageType, message) {
        const msg = {
            MessageType: messageType,
            Data: JSON.stringify(message)
        };

        socket.send(JSON.stringify(msg));
    }
