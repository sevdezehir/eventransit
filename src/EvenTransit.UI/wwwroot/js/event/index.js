const serviceModal = new bootstrap.Modal(document.getElementById('newEventModal'), {});
const saveForm = document.querySelector("#save-event");
document.querySelectorAll('.delete-event')
    .forEach(button => button.addEventListener('click', deleteEvent));

saveForm.addEventListener("submit", async e => {
    e.stopPropagation();
    e.preventDefault();

    const formData = Object.fromEntries(new FormData(e.target).entries());

    const response = await fetch('/Events/SaveEvent', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
    });

    const result = await response.json();
    document.getElementById("event-errors").classList.add("d-none");

    if (!result.isSuccess) {

        document.getElementById("event-errors").innerHTML = result.message;
        document.getElementById("event-errors").classList.remove("d-none");

        setTimeout(function () {
            document.getElementById("event-errors").classList.add("d-none");
            document.getElementById("event-errors").innerHTML = "";
        }, 5000);
        return;
    }

    serviceModal.hide();
    window.location.reload();
});

async function deleteEvent(e) {
    if (confirm("Are you sure?")) {
        const id = e.currentTarget.dataset.id;

        const response = await fetch(`/Events/Delete/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.status !== 200) {
            alert("Event not deleted!");
            return;
        }

        window.location.reload();
    }
}