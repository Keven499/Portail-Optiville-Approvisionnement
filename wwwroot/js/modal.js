var myModalElement = document.getElementById('exampleModalCenter');
var myModal = new bootstrap.Modal(myModalElement);

myModalElement.addEventListener('hidden.bs.modal', function () {
    var backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) {
        backdrop.remove(); // Remove the backdrop element
    }
});

function hideMyModal() {
    myModal.hide();
}
