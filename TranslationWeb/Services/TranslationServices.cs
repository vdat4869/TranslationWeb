using Azure.AI.Translation.Text;
using Microsoft.CognitiveServices.Speech;
using Tesseract;

public class TranslationServices
{
    private readonly TextTranslationClient _translationClient;
    private readonly string _speechKey;
    private readonly string _speechRegion;

    public TranslationServices(string endpoint, string translationKey, string speechKey, string speechRegion)
    {
        _translationClient = new TextTranslationClient(
            new Azure.AzureKeyCredential(translationKey),
            new Uri(endpoint));
        _speechKey = speechKey;
        _speechRegion = speechRegion;
    }

    public async Task<(string translatedText, string? detectedLanguage)> TranslateTextAsync(
        string text, string targetLanguage, string? sourceLanguage = null)
    {
        try
        {
            var response = await _translationClient.TranslateAsync(
                targetLanguage,
                text,
                sourceLanguage: sourceLanguage == "auto" ? null : sourceLanguage);

            var translation = response.Value.FirstOrDefault();
            return (translation?.Translations.FirstOrDefault()?.Text ?? string.Empty,
                    translation?.DetectedLanguage?.Language);
        }
        catch (Exception ex)
        {
            throw new Exception($"Translation failed: {ex.Message}");
        }
    }

    public async Task<byte[]> TextToSpeechAsync(string text, string language)
    {
        var config = SpeechConfig.FromSubscription(_speechKey, _speechRegion);
        config.SpeechSynthesisLanguage = language;

        using var synthesizer = new SpeechSynthesizer(config);
        using var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            return result.AudioData;
        }

        throw new Exception($"Speech synthesis failed: {result.Reason}");
    }

    public async Task<(string extractedText, string translatedText)> TranslateImageAsync(
        Stream imageStream, string targetLanguage)
    {
        // Save stream to temporary file
        var tempImagePath = Path.GetTempFileName();
        using (var fileStream = File.Create(tempImagePath))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        try
        {
            // Initialize Tesseract
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(tempImagePath);
            using var page = engine.Process(img);

            var extractedText = page.GetText().Trim();

            // Translate extracted text
            var (translatedText, _) = await TranslateTextAsync(extractedText, targetLanguage);

            return (extractedText, translatedText);
        }
        finally
        {
            File.Delete(tempImagePath);
        }
    }
} 