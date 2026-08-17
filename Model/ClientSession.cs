using System;
using System.Collections.Generic;

namespace _4RTools.Model
{
    public sealed class ClientSession
    {
        public Client Client { get; private set; }
        public Profile Profile { get; set; }
        public string ProfileName { get { return Profile == null ? "Default" : Profile.Name; } }
        public bool IsRunning { get; private set; }

        public ClientSession(Client client, Profile profile)
        {
            Client = client;
            Profile = profile;
        }

        public void Start()
        {
            if (IsRunning || Profile == null || Client == null) { return; }

            ClientSingleton.Instance(Client);
            StartProfileActions(Profile);
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning || Profile == null) { return; }

            StopProfileActions(Profile);
            IsRunning = false;
        }

        private static void StartProfileActions(Profile profile)
        {
            profile.AHK.Start();
            profile.Autopot.Start();
            profile.AutopotYgg.Start();
            profile.Autobuff.Start();
            profile.StatusRecovery.Start();
            profile.DebuffsRecovery.Start();
            profile.AutoRefreshSpammer1.Start();
            profile.AutoRefreshSpammer2.Start();
            profile.AutoRefreshSpammer3.Start();
            profile.SongMacro.Start();
            profile.MacroSwitch.Start();
            profile.AtkDefMode.Start();
            profile.PixelMacro.Start();
        }

        private static void StopProfileActions(Profile profile)
        {
            profile.AHK.Stop();
            profile.Autopot.Stop();
            profile.AutopotYgg.Stop();
            profile.Autobuff.Stop();
            profile.StatusRecovery.Stop();
            profile.DebuffsRecovery.Stop();
            profile.AutoRefreshSpammer1.Stop();
            profile.AutoRefreshSpammer2.Stop();
            profile.AutoRefreshSpammer3.Stop();
            profile.SongMacro.Stop();
            profile.MacroSwitch.Stop();
            profile.AtkDefMode.Stop();
            profile.PixelMacro.Stop();
        }
    }

    public static class ClientSessionManager
    {
        private static readonly Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>();

        public static ClientSession Selected { get; private set; }

        public static IEnumerable<ClientSession> Sessions { get { return sessions.Values; } }

        public static ClientSession Select(Client client)
        {
            if (client == null || client.process == null) { return null; }

            ClientSession session;
            if (!sessions.TryGetValue(client.process.Id, out session))
            {
                session = new ClientSession(client, ProfileSingleton.LoadProfile("Default"));
                sessions.Add(client.process.Id, session);
            }
            else
            {
                session = sessions[client.process.Id];
            }

            Selected = session;
            ClientSingleton.Instance(session.Client);
            ProfileSingleton.Use(session.Profile);
            return session;
        }

        public static void SetSelectedProfile(Profile profile)
        {
            if (Selected == null || profile == null) { return; }

            bool restart = Selected.IsRunning;
            if (restart) { Selected.Stop(); }
            Selected.Profile = profile;
            ProfileSingleton.Use(profile);
            if (restart) { Selected.Start(); }
        }

        public static void StopAll()
        {
            foreach (ClientSession session in sessions.Values)
            {
                session.Stop();
            }
        }
    }
}
