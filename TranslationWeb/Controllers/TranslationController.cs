using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class TranslationController : ControllerBase
{
    private readonly TranslationServices _translationServices;

    public TranslationController(TranslationServices translationServices)
    {
        _translationServices = translationServices;
    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
    {
        try
        {
            var (translatedText, detectedLanguage) = await _translationServices.TranslateTextAsync(
                request.Text,
                request.TargetLanguage,
                request.SourceLanguage);

            return Ok(new
            {
                translatedText,
                detectedLanguage
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("speak")]
    public async Task<IActionResult> Speak([FromBody] SpeechRequest request)
    {
        try
        {
            var audioData = await _translationServices.TextToSpeechAsync(
                request.Text,
                request.Language);

            return File(audioData, "audio/wav");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("translateImage")]
    public async Task<IActionResult> TranslateImage([FromForm] IFormFile image, [FromForm] string targetLanguage)
    {
        try
        {
            using var stream = image.OpenReadStream();
            var (extractedText, translatedText) = await _translationServices.TranslateImageAsync(
                stream,
                targetLanguage);

            return Ok(new
            {
                extractedText,
                translatedText
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class TranslationRequest
{
    public string Text { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
}

public class SpeechRequest
{
    public string Text { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}