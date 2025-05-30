document.addEventListener('DOMContentLoaded', function() {
    const sourceText = document.getElementById('sourceText');
    const targetText = document.getElementById('targetText');
    const sourceLanguage = document.getElementById('sourceLanguage');
    const targetLanguage = document.getElementById('targetLanguage');
    const swapLanguages = document.getElementById('swapLanguages');
    const speakSource = document.getElementById('speakSource');
    const speakTarget = document.getElementById('speakTarget');
    const imageUpload = document.getElementById('imageUpload');
    const imagePreview = document.getElementById('imagePreview');

    let translationTimeout;

    // Translation function
    async function translateText() {
        const text = sourceText.value;
        if (!text) {
            targetText.value = '';
            return;
        }

        try {
            const response = await fetch('/api/translate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    text: text,
                    sourceLanguage: sourceLanguage.value,
                    targetLanguage: targetLanguage.value
                })
            });

            const data = await response.json();
            targetText.value = data.translatedText;
            
            if (sourceLanguage.value === 'auto') {
                // Update source language dropdown with detected language
                sourceLanguage.value = data.detectedLanguage;
            }
        } catch (error) {
            console.error('Translation error:', error);
            targetText.value = 'Error during translation';
        }
    }

    // Debounced translation
    function debouncedTranslate() {
        clearTimeout(translationTimeout);
        translationTimeout = setTimeout(translateText, 500);
    }

    // Text-to-speech function
    async function speakText(text, language) {
        try {
            const response = await fetch('/api/speak', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    text: text,
                    language: language
                })
            });

            const audioBlob = await response.blob();
            const audioUrl = URL.createObjectURL(audioBlob);
            const audio = new Audio(audioUrl);
            audio.play();
        } catch (error) {
            console.error('Speech error:', error);
        }
    }

    // Image translation function
    async function translateImage(file) {
        const formData = new FormData();
        formData.append('image', file);
        formData.append('targetLanguage', targetLanguage.value);

        try {
            const response = await fetch('/api/translateImage', {
                method: 'POST',
                body: formData
            });

            const data = await response.json();
            sourceText.value = data.extractedText;
            targetText.value = data.translatedText;
        } catch (error) {
            console.error('Image translation error:', error);
        }
    }

    // Event listeners
    sourceText.addEventListener('input', debouncedTranslate);
    
    sourceLanguage.addEventListener('change', translateText);
    targetLanguage.addEventListener('change', translateText);

    swapLanguages.addEventListener('click', () => {
        if (sourceLanguage.value !== 'auto') {
            const temp = sourceLanguage.value;
            sourceLanguage.value = targetLanguage.value;
            targetLanguage.value = temp;
            
            const tempText = sourceText.value;
            sourceText.value = targetText.value;
            targetText.value = tempText;
            
            translateText();
        }
    });

    speakSource.addEventListener('click', () => {
        speakText(sourceText.value, sourceLanguage.value);
    });

    speakTarget.addEventListener('click', () => {
        speakText(targetText.value, targetLanguage.value);
    });

    imageUpload.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (file) {
            // Show image preview
            const reader = new FileReader();
            reader.onload = function(e) {
                imagePreview.innerHTML = `<img src="${e.target.result}" alt="Uploaded image">`;
            };
            reader.readAsDataURL(file);

            // Translate image
            translateImage(file);
        }
    });
}); 