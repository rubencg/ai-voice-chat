using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoiceChatApp.Services
{
    internal interface IGptChatter
    {
        IList<ChatMessage> Messages { get; }

        Task<string?> Talk(string phrase);
    }

    public class IronManChatter : IGptChatter
    {
        public IList<ChatMessage> Messages { get; private set; }
        private OpenAIService _openAIService;

        public IronManChatter()
        {
            _openAIService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = "<key here>"
            });
            Messages = new List<ChatMessage>()
            {
                //ChatMessage.FromSystem("Act as if you were Iron man from the Marvel movies. You will have a conversation with me, Ruben, a software engineer, about how you built your suit. The conversation has be in french all the time. Keep answers short, maximum two lines."),
                //ChatMessage.FromAssistant("Salut Ruben, voulez-vous savoir comment j'ai construit mon costume?")
                ChatMessage.FromSystem("Act as a Ezio Auditore, have a conversation in french with me. The conversation has be in french all the time. Keep answers short, maximum two or three lines. Keep the conversation flowing"),
                ChatMessage.FromUser("Salut Ezio!"),
                ChatMessage.FromAssistant("Salut!")
            };
        }

        public async Task<string?> Talk(string phrase)
        {
            Messages.Add(ChatMessage.FromUser(phrase));
            var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = Messages,
                Model = Models.ChatGpt3_5Turbo
            });

            if (completionResult.Successful)
            {
                var response = completionResult.Choices.First().Message.Content;
                Messages.Add(ChatMessage.FromAssistant(phrase));
                return response;
            }

            return null;
        }
    }
}
