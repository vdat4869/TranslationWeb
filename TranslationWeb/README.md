# Multilingual Translation Website

A web application that provides translation services with the following features:
- Language switching between two languages
- Automatic language detection
- Image text translation
- Text-to-speech pronunciation
- No login required

## Requirements
- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- Azure Cognitive Services API Key (for translation and speech services)
- Tesseract OCR (for image text recognition)

## Setup Instructions
1. Clone the repository
2. Create a `appsettings.json` file and add your Azure Cognitive Services API key
3. Install required NuGet packages
4. Run the application using `dotnet run`

## NuGet Packages Used
- Microsoft.CognitiveServices.Speech
- Azure.AI.Translation.Text
- Tesseract
- System.Drawing.Common 