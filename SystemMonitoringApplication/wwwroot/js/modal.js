var modal = document.getElementById("highLoadModal");
function viewModal(data) {
    var modalMessage = document.getElementById("modalMessage");
    modalMessage.innerText = `HighLoad system state: TotalCpuUsagePercent(${data.systemInfo.totalCpuUsagePercent}) TotalMemoryUsagePercent(${data.systemInfo.totalMemoryUsagePercent}) TotalMemoryUsageMb(${data.systemInfo.totalMemoryUsageMb})`;
    var modal = document.getElementById("highLoadModal");
    modal.style.display = "block";
}

var span = document.getElementsByClassName("close")[0];

span.onclick = function() {
    modal.style.display = "none";
};

window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}