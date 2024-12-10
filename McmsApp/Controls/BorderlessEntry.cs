using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace McmsApp.Controls
{
    /// <summary>
    /// This class is inherited from Xamarin.Forms.Entry to remove the border for Entry control in the Android platform.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class BorderlessEntry : Entry
    {
    }
}
