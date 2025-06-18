const searchInput = document.getElementById('searchInput');
if (searchInput) {
    searchInput.addEventListener('input', function() {
        document.getElementById('hiddenSearchText').value = this.value;
        document.getElementById('combinedForm').submit();
    });
}

const resultFrame = document.getElementById('resultFrame');
if (resultFrame && !resultFrame.src) {
    resultFrame.src = '/LogTable?isPartial=true';
}

document.querySelectorAll('.sidebar-filter-title').forEach(filterTitle => {
    filterTitle.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();

        const filterName = this.dataset.filter;
        const toggleLink = this.querySelector('.filter-toggle-link');
        const filterContent = this.parentElement.querySelector('.sidebar-filter-content');

        toggleLink.classList.toggle('open');
        filterContent.classList.toggle('show');

        let expandedFilters = [];

        document.querySelectorAll('input[name="ExpandedFilters"]').forEach(input => {
            input.remove();
        });

        document.querySelectorAll('.sidebar-filter-content.show').forEach(content => {
            const filterTitle = content.previousElementSibling;
            if (filterTitle && filterTitle.dataset.filter) {
                expandedFilters.push(filterTitle.dataset.filter);
            }
        });

        expandedFilters.forEach(filter => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'ExpandedFilters';
            input.value = filter;
            document.getElementById('combinedForm').appendChild(input);
        });
    });
});
if (searchInput) {
    searchInput.addEventListener('input', function() {
        document.getElementById('hiddenSearchText').value = this.value;
        document.getElementById('combinedForm').submit();
    });
}

if (resultFrame && !resultFrame.src) {
    resultFrame.src = '/LogTable?isPartial=true';
}

document.querySelectorAll('.sidebar-filter-title').forEach(filterTitle => {
    filterTitle.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();

        const filterName = this.dataset.filter;
        const toggleLink = this.querySelector('.filter-toggle-link');
        const filterContent = this.parentElement.querySelector('.sidebar-filter-content');

        toggleLink.classList.toggle('open');
        filterContent.classList.toggle('show');

        let expandedFilters = [];

        document.querySelectorAll('input[name="ExpandedFilters"]').forEach(input => {
            input.remove();
        });

        document.querySelectorAll('.sidebar-filter-content.show').forEach(content => {
            const filterTitle = content.previousElementSibling;
            if (filterTitle && filterTitle.dataset.filter) {
                expandedFilters.push(filterTitle.dataset.filter);
            }
            
            toggleLink.classList.toggle('open');
            filterContent.classList.toggle('show');
            
            let expandedFilters = [];
            
            document.querySelectorAll('input[name="ExpandedFilters"]').forEach(input => {
                input.remove();
            });
            
            document.querySelectorAll('.sidebar-filter-content.show').forEach(content => {
                const filterTitle = content.previousElementSibling;
                if (filterTitle && filterTitle.dataset.filter) {
                    expandedFilters.push(filterTitle.dataset.filter);
                }
            });
            
            expandedFilters.forEach(filter => {
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'ExpandedFilters';
                input.value = filter;
                document.getElementById('combinedForm').appendChild(input);
            });
        });
    });
