using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceChatApp.Services
{
    public interface IAudioRecorder
    {
        void StartRecordingAudio();
        string StopAndReturnPathToAudioFile();
    }

    public class AudioRecorder : IAudioRecorder
    {
        private WaveInEvent _waveIn;
        private WaveFileWriter _waveFileWriter;
        private string _outputFilePath;

        public void StartRecordingAudio()
        {
            string fileName = $"audio_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            _outputFilePath = Path.Combine(Environment.CurrentDirectory, fileName);
            _waveIn = new WaveInEvent();
            _waveIn.DataAvailable += WaveIn_DataAvailable;
            _waveIn.RecordingStopped += WaveIn_RecordingStopped;
            _waveFileWriter = new WaveFileWriter(fileName, _waveIn.WaveFormat);
            _waveIn.StartRecording();
        }

        public string StopAndReturnPathToAudioFile()
        {
            _waveIn.StopRecording();
            _waveIn.DataAvailable -= WaveIn_DataAvailable;
            _waveIn.RecordingStopped -= WaveIn_RecordingStopped;

            _waveFileWriter.Dispose();

            return _outputFilePath;
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            _waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
            if (_waveFileWriter.Position > _waveIn.WaveFormat.AverageBytesPerSecond * 30)
            {
                _waveIn.StopRecording();
            }
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            _waveFileWriter?.Dispose();
            _waveFileWriter = null;
            _waveIn.Dispose();
        }
    }
}
