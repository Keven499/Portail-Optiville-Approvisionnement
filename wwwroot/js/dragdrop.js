window.dragAndDrop = {
    initialize: function (dropAreaId, fileInputId) {
        const dropArea = document.getElementById(dropAreaId);
        const fileInput = document.getElementById(fileInputId);

        // Prevent default behavior (Prevent file from being opened)
        dropArea.addEventListener('dragover', (e) => {
            e.preventDefault();
            dropArea.classList.add('dragging'); // Add visual cue
        });

        dropArea.addEventListener('dragleave', () => {
            dropArea.classList.remove('dragging'); // Remove visual cue
        });

        dropArea.addEventListener('drop', (e) => {
            e.preventDefault();
            dropArea.classList.remove('dragging'); // Remove visual cue

            // Handle file drop and pass it to the Blazor component
            const files = e.dataTransfer.files;
            if (files.length > 0) {
                const singleFile = new DataTransfer();
                singleFile.items.add(files[0]); // Add only the first file

                // Set the single file to the input and trigger change
                fileInput.files = singleFile.files;
                fileInput.dispatchEvent(new Event('change')); // Trigger the change event
            }
        });
    }
};
