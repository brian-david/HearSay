//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Text;
using Xamarin.Forms;
using Microsoft.CognitiveServices.Speech;
using Xamarin.Forms.PancakeView;
using System.Collections.Generic;
using HearSay.Models;
using HearSay.Services;


namespace HearSay
{
    public class User
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public partial class MainPage : ContentPage
    {
        //VARIABLES AND DATA
        bool listening = false;

        User currentUser = new User {Name = ""};
        private List<string> phrases;

        public MainPage()
        {
            InitializeComponent();
            phrases = new List<string>();
        }

        private async void OnRecognitionButtonClicked(object sender, EventArgs e)
        {
            //ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = false };
            //AI_LIS.IsRunning = true;
            listening = true;
            ListenBtn.Text = "listening";           
            ListenBtn.BackgroundColor = Color.Green;

            //Check if the app has microphone permissions
            bool micAccessGranted = await DependencyService.Get<IMicrophoneService>().GetPermissionsAsync();
            if (!micAccessGranted)
            {
                UpdateUI("Please give access to microphone");
            }
            else
            {
                Console.WriteLine("mic access granted");
            }

            try
            {
                // Creates an instance of a speech config with specified subscription key and service region.
                // Replace with your own subscription key and service region (e.g., "westus").
                var config = SpeechConfig.FromSubscription("fa287654e33b43e8a92abe5e40beef4f", "westus2");

                // Creates a speech recognizer using microphone as audio input.
                using (var recognizer = new SpeechRecognizer(config))
                {

                    // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                    // single utterance is determined by listening for silence at the end or until a maximum of 15
                    // seconds of audio is processed.  The task returns the recognition text as result.
                    // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                    // shot recognition like command or query.
                    // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                    var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    // Checks result.
                    StringBuilder sb = new StringBuilder();
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        sb.AppendLine($"{result.Text}");
                        phrases.Add(result.Text);
                        Console.WriteLine("This is what I heard: " + result.Text);
                        foreach (var phrase in phrases)
                        {
                            Console.WriteLine(phrase);
                        }

                    }
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        sb.AppendLine($"NOMATCH: Speech could not be recognized.");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result);
                        sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            sb.AppendLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            sb.AppendLine($"CANCELED: Did you update the subscription info?");
                        }
                    }

                    if (sb.ToString().Contains(currentUser.Name))
                    {
                        //var tone = await SentimentAnalysis.GetSentiment("Tony is a great man");
                        //Console.WriteLine("SENTIMENT ANALYSIS -> "+tone);

                        UpdateUI(sb.ToString());
                    }
                    else {
                        UpdateUI("No one is talking about you");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                UpdateUI("Exception: " + ex.ToString());
            }

            Console.WriteLine("SENTIMENT SCORE -> "+await TextAnalysisService.GetSentiment("I fucking hate you"));
        }

        private void UpdateUI(String message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //AI_LIS.IsRunning = false;
                listening = false;
                ListenBtn.Text = "listen";
                ListenBtn.BackgroundColor = Color.FromHex("#2E74D1");

                var textBox = new PancakeView {
                    BackgroundColor = Color.WhiteSmoke,
                    CornerRadius = 10,
                    HasShadow = true
                };

                var sentiment = await TextAnalysisService.GetSentiment(message);
                textBox.BackgroundColor = SetSpeechColour((double)sentiment);
                textBox.Content = new Label { Text = message, Margin = 7 };
                speech.Children.Add(textBox);
            });
        }

        async void SettingsPage(object sender, EventArgs e)
        {
            var settingsPage = new SettingsPage();
            settingsPage.BindingContext = currentUser;
            Console.WriteLine("CURRENT NAME -> " + currentUser.Name);
            await Navigation.PushAsync(settingsPage, true);
        }

        private Color SetSpeechColour(double result)
        {
            switch (result)
            {
                case double number when (number <= 0.2):
                    return Color.FromHex("#D93B3B");
                case double number when (number > 0.2 && number <= 0.4):
                    return Color.FromHex("#731A1A");
                case double number when (number > 0.4 && number < 0.6):
                    return Color.WhiteSmoke;
                case double number when (number >= 0.6 && number < 0.8):
                    return Color.FromHex("#358C6C");
                case double number when (number >= 0.8):
                    return Color.FromHex("#49BF94");
                default:
                    return Color.WhiteSmoke;
            }
            //return Color.Blue;
        }
    }
}
