using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HearSay
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        async void submitName(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
