using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using VoiceChatApp.Services;
using System.Linq;

namespace VoiceChatApp
{
    public partial class MainWindow : Window
    {
        private ISpeechToTextService _speechToTextService;
        private IAudioRecorder _audioRecorder;
        private SpeechSynthesizer _speechSynthesizer;
        private IGptChatter _gptChatter;



        public MainWindow()
        {
            InitializeComponent();
            _speechToTextService = new SpeechToTextService();
            _audioRecorder = new AudioRecorder();
            // Create a new instance of the SpeechSynthesizer class
            _speechSynthesizer = new SpeechSynthesizer();

            // Set the voice to use (optional)
            _speechSynthesizer.SelectVoice("Microsoft Paul");

            // Set the output audio format (optional)
            _speechSynthesizer.SetOutputToDefaultAudioDevice();

            _gptChatter = new IronManChatter();

            foreach (var message in _gptChatter.Messages)
            {
                inputTextBox.Text = message.Content.ToString() + "\n";
            }
        }


        private void MicrophoneToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            microphoneToggleButton.Content = "Recording...";
            _audioRecorder.StartRecordingAudio();
        }

        private async void MicrophoneToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            microphoneToggleButton.Content = "Microphone";
            await ProcessAudioWithWhisper(_audioRecorder.StopAndReturnPathToAudioFile());
        }

        private async Task ProcessAudioWithWhisper(string audioPath)
        {
            var textFromAudioFromUser = await _speechToTextService.GetTextFromAudioFile(audioPath);
            
            // Add text from user to textbox
            inputTextBox.Text += textFromAudioFromUser + "\n";

            // Delete file as it won't be needed anymore
            File.Delete(audioPath);

            var responseFromChatGpt = await _gptChatter.Talk(textFromAudioFromUser);

            if(responseFromChatGpt != null)
            {
                inputTextBox.Text += responseFromChatGpt + "\n";
                _speechSynthesizer.SpeakAsync(responseFromChatGpt);
            }
        }

       

    }
}
