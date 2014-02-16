#if PLATFORM_ANDROID || PLATFORM_OUYA

namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using Android.Content.PM;
    
    using Microsoft.Xna.Framework;
    
    using Ninject;
    
    using Protogame;
  
    [Activity(
        Label = "Mir",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class MirActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var kernel = new StandardKernel();
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, null);

            // Create our OpenGL view, and display it
            MirGame.Activity = this;
            var g = new MirGame(kernel);
            SetContentView(g.AndroidGameWindow);
            g.Run();
        }
    }
}

#endif
