const uri = "api/ProcessStates";

fetch(uri)
    .then(response => response.json())
    .then(data => displayItems(data))
    .catch(error => console.error("Unable to get items.", error));


function insertProcessState(processState) {
    const tBody = document.getElementById("processStates");
    const  tr = tBody.insertRow();
    appendProcessState(tr, processState);
}

function updateProcessState(processState) {
    const tr = document.getElementById(processState.processId);
    tr.innerHTML = "";
    appendProcessState(tr, processState);
}

function removeProcess(processState) {
    const element = document.getElementById(processState.processId);
    element.parentNode.removeChild(element);
}

function displayItems(data) {

    const tBody = document.getElementById("processStates");

    tBody.innerHTML = "";

    data.forEach(item => {
        const  tr = tBody.insertRow();
        appendProcessState(tr, item);
    });
}

function appendProcessState(tr, processState) {

    tr.setAttribute("id", processState.processId);

    const td1 = tr.insertCell(0);
    td1.appendChild(document.createTextNode(processState.processId));

    const td2 = tr.insertCell(1);
    td2.appendChild(document.createTextNode(processState.processName));

    const td3 = tr.insertCell(2);
    td3.appendChild(document.createTextNode(processState.cpu));

    const td4 = tr.insertCell(3);
    td4.appendChild(document.createTextNode(processState.memory));
}