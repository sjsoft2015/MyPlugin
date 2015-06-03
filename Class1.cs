using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using NLog;

using Sandbox.Game.Entities.Character;
using Sandbox.ModAPI;

using SEModAPIExtensions.API;
using SEModAPIExtensions.API.Plugin;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace MyPlugin
{
    public class MyPlugin : IPlugin
    {
        public static Logger Log;
        private static MyPlugin _instance;
        public static string CharacterNamespace = "Sandbox.Game.Entities.Character";
        public static string CharacterClass = "MyCharacter";

        public void Init()
        {
            PluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Log.Info("Initializing MyPlugin plugin at path {0}", PluginPath);
            _instance = this;
        }

        public void Update()
        {
            try
            {
                // Get list of spawned players
                List<IMyPlayer> connectedPlayers = new List<IMyPlayer>();
                HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
                MyAPIGateway.Entities.GetEntities(entities);
                MyAPIGateway.Players.GetPlayers(connectedPlayers, x => x.Controller != null && x.Controller.ControlledEntity != null);

                foreach (IMyEntity entity in entities)
                {
                    if (entity is MyCharacter)
                    {
                        MyCharacter pl = (MyCharacter)entity;
                        if (!pl.IsDead && pl.IsJetpackPowered())
                        {
                            ChatManager.Instance.SendPublicChatMessage(string.Format("Jetpack powered: {0}, JetpackEnabled: {1}", pl.IsJetpackPowered(), pl.JetpackEnabled));
                            Log.Info("Jetpack powered: {0}, JetpackEnabled: {1}", pl.IsJetpackPowered(), pl.JetpackEnabled);
                            pl.SwitchThrusts();
                            //pl.EnableJetpack(false, false, true, false);
                            //Type m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CharacterNamespace, CharacterClass);
                            //MethodInfo method = entity.GetType().GetMethod("EnableJetpack");
                            //SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                            //{
                            //    Object result = method.Invoke(entity, new object[] { false, false, true, false });
                            //});
                            ChatManager.Instance.SendPublicChatMessage(string.Format("Jetpack powered: {0}, JetpackEnabled: {1}", pl.IsJetpackPowered(), pl.JetpackEnabled));
                            Log.Info("Jetpack powered: {0}, JetpackEnabled: {1}", pl.IsJetpackPowered(), pl.JetpackEnabled);
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
                Log.Info(ex.ToString());
            }


        }

        public void Shutdown()
        {
            Log.Info("Shutting down MyPlugin - {0}", Version);
        }

        internal static MyPlugin Instance
        {
            get
            {
                Log.Info("MyPlugin Instance");
                return _instance ?? (_instance = new MyPlugin());
            }
        }

        public Guid Id
        {
            get
            {
                Log.Info("Guid Id");
                GuidAttribute guidAttr = (GuidAttribute)typeof(MyPlugin).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                return new Guid(guidAttr.Value);
            }
        }

        public string Name { get { return "MyPlugin"; } }

        public Version Version { get { return typeof(MyPlugin).Assembly.GetName().Version; } }

        public static string PluginPath { get; set; }
    }

}
