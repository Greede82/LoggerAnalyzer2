document.addEventListener('DOMContentLoaded', function() {
    const filterTitles = document.querySelectorAll('.sidebar-filter-title');
    filterTitles.forEach(title => {
        title.addEventListener('click', function() {
            const filter = this.getAttribute('data-filter');
            const content = this.nextElementSibling;
            const toggleLink = this.querySelector('.filter-toggle-link');
            content.classList.toggle('show');
            toggleLink.classList.toggle('open');
            addOrRemoveExpandedFilter(filter, content.classList.contains('show'));
        });
    });
});

function addOrRemoveExpandedFilter(filterName, isExpanded) {
    fetch(`/LogTable?handler=ToggleFilter&filterName=${filterName}&isPartial=true`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
    })
    .then(response => response.text())
    .then(html => {
        document.getElementById('resultFrame').srcdoc = html;
    })
    .catch(error => console.error('Error:', error));
}
