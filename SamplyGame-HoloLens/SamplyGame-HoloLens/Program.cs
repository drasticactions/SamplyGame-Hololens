using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.HoloLens;
using Urho.Physics;
using Urho.Shapes;

namespace SamplyGame_HoloLens
{
    /// <summary>
    /// Windows Holographic application using SharpDX.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            CoreApplication.Run(new AppViewSource());
        }

        class AppViewSource : IFrameworkViewSource
        {
            public IFrameworkView CreateView() => UrhoAppView.Create<SamplyGame>("Data");
        }
    }

    public class SamplyGame : HoloApplication
    {
        int coins;
        Node textNode;
        Node rotor;
        Node bigAircraft;
        Node menuLight;

        public Viewport Viewport { get; private set; }

        public SamplyGame(string assetsDirectory) : base(assetsDirectory)
        {
        }

        protected override async void Start()
        {
            // base.Start() creates a basic scene
            base.Start();
            CreateScene();
        }

        private async void CreateScene()
        {
            var cache = ResourceCache;
            bigAircraft = Scene.CreateChild();

            var model = bigAircraft.CreateComponent<StaticModel>();
            model.Model = cache.GetModel(Assets.Models.Player);
            model.SetMaterial(cache.GetMaterial(Assets.Materials.Player).Clone(""));

            bigAircraft.Position = new Vector3(0, 0, 1);
            bigAircraft.SetScale(0.2f);

            bigAircraft.Position = new Vector3(10, 2, 10);
            bigAircraft.RunActions(new RepeatForever(new Sequence(new RotateBy(1f, 0f, 0f, 5f), new RotateBy(1f, 0f, 0f, -5f))));

            //TODO: rotor should be defined in the model + animation
            rotor = bigAircraft.CreateChild();
            var rotorModel = rotor.CreateComponent<Box>();
            rotorModel.Color = Color.White;
            rotor.Scale = new Vector3(0.1f, 1.5f, 0.1f);
            rotor.Position = new Vector3(0, 0, -1.3f);
            var rotorAction = new RepeatForever(new RotateBy(1f, 0, 0, 360f * 6)); //RPM
            rotor.RunActions(rotorAction);

            DirectionalLight.Brightness -= 0.6f;

            await bigAircraft.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 2));

        }

        protected override void OnUpdate(float timeStep)
        {
            //For HL optical stabilization (optional)
            FocusWorldPoint = bigAircraft.WorldPosition;
        }
    }
}