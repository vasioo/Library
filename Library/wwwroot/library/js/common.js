var commonFuncs = (function () {
    function validateAndResizeImage(file, callback) {

        const maxSizeInPixels = 1024, // Maximum dimension in pixels
            maxSizeInBytes = 1 * 1024 * 1024; // 1MB in bytes

        if (file.type.includes('image/')) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const image = new Image();
                image.onload = function () {
                    if (file.size <= maxSizeInBytes && (image.width > maxSizeInPixels || image.height > maxSizeInPixels)) {
                        resizeImage(image, maxSizeInPixels, function (resizedDataUrl) {
                            if (getImageSizeInBytes(resizedDataUrl) <= maxSizeInBytes) {
                                callback(true, resizedDataUrl);
                            } else {
                                callback(false, 'Resized image size exceeds the maximum limit of 5MB.');
                            }
                        });
                    } else {
                        callback(true, e.target.result);
                    }
                };
                image.src = e.target.result;
            };
            reader.readAsDataURL(file);
        } else {
            callback(false, 'Invalid file type.');
        }
    }


    function resizeImage(image, maxSize, callback) {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        let width = image.width;
        let height = image.height;

        if (width > height) {
            if (width > maxSize) {
                height *= maxSize / width;
                width = maxSize;
            }
        } else {
            if (height > maxSize) {
                width *= maxSize / height;
                height = maxSize;
            }
        }

        canvas.width = width;
        canvas.height = height;

        ctx.drawImage(image, 0, 0, width, height);

        const resizedDataUrl = canvas.toDataURL('image/jpeg'); // Adjust the format as needed

        callback(resizedDataUrl);
    }



    function getImageSizeInBytes(dataUrl) {
        const base64String = dataUrl.split(',')[1],
            padding = (base64String.length % 4 === 0 ? 0 : 4 - (base64String.length % 4)),
            base64 = base64String + '='.repeat(padding),
            sizeInBytes = (base64.length * 0.75) - (padding);

        return sizeInBytes;
    }

    // Text validation for the subcategory id
    function isValidIdText(text) {

        let regex = /^[a-zA-Z][a-zA-Z0-9-_:.]*$/,
            wordsInText = text.split(' ').length;

        text = text.replaceAll(' ', '_');

        if (text.length > 20) {
            alert('The text in the input can not be over 20 characters!');
            return false;
        }
        else if (wordsInText > 3) {
            alert('There can not be more than 3 words in a subcategory name!');
            return false;
        }
        else if (!regex.test(text)) {
            alert('Only alphanumeric characters are allowed!');
            return false;
        }
        return true;
    }

    return {
        validateAndResizeImage,
        resizeImage,
        isValidIdText
    };

})();