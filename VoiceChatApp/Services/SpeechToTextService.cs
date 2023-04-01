using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VoiceChatApp.Model;
using System.IO;

namespace VoiceChatApp.Services
{
    public interface ISpeechToTextService
    {
        Task<string> GetTextFromAudioFile(string pathToaudioFile);
    }

    public class SpeechToTextService : ISpeechToTextService
    {
        public async Task<string> GetTextFromAudioFile(string pathToaudioFile)
        {
            using HttpClient httpClient = new HttpClient();
            using MultipartFormDataContent form = new MultipartFormDataContent();

            // Read the audio file into a byte array.
            byte[] fileBytes = File.ReadAllBytes(pathToaudioFile);

            // Add the file content, model, and language to the form.
            form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", Path.GetFileName(pathToaudioFile));
            form.Add(new StringContent("whisper-1"), "model");
            form.Add(new StringContent("fr"), "language");

            // Set the Authorization header.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");

            // Send the POST request and get the response.
            HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);

            // Check if the request was successful.
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string.
                string responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize
                var speechResponse = JsonConvert.DeserializeObject<SpeechResponse>(responseContent);

                // Process the response content and update the textbox.
                // You may need to parse the JSON response to extract the text, depending on the API's response format.
                return speechResponse?.Text;
            }

            throw new Exception($"Error while retrieving text from audio file {pathToaudioFile}");
        }
    }
}
