using System.Collections.Generic;
using Newtonsoft.Json;
using _4RTools.Utils;
using _4RTools.Forms;
using System.IO;
using System;

namespace _4RTools.Model
{
    public class ProfileSingleton
    {
        public static Profile profile = new Profile("Default");

        public static void Load(string profileName)
        {
            profile = LoadProfile(profileName);
        }

        public static Profile LoadProfile(string profileName)
        {
            try
            {
                string json = File.ReadAllText(AppConfig.ProfileFolder + profileName + ".json");
                dynamic rawObject = JsonConvert.DeserializeObject(json);
                Profile loadedProfile = new Profile(profileName);

                if ((rawObject != null))
                {
                    loadedProfile.UserPreferences = JsonConvert.DeserializeObject<UserPreferences>(Profile.GetByAction(rawObject, loadedProfile.UserPreferences));
                    loadedProfile.AHK = JsonConvert.DeserializeObject<AHK>(Profile.GetByAction(rawObject, loadedProfile.AHK));
                    loadedProfile.Autopot = JsonConvert.DeserializeObject<Autopot>(Profile.GetByAction(rawObject, loadedProfile.Autopot));
                    loadedProfile.AutopotYgg = JsonConvert.DeserializeObject<Autopot>(Profile.GetByAction(rawObject, loadedProfile.AutopotYgg));
                    loadedProfile.StatusRecovery = JsonConvert.DeserializeObject<StatusRecovery>(Profile.GetByAction(rawObject, loadedProfile.StatusRecovery));
                    loadedProfile.AutoRefreshSpammer1 = JsonConvert.DeserializeObject<AutoRefreshSpammer>(Profile.GetByAction(rawObject, loadedProfile.AutoRefreshSpammer1));
                    loadedProfile.AutoRefreshSpammer2 = JsonConvert.DeserializeObject<AutoRefreshSpammer>(Profile.GetByAction(rawObject, loadedProfile.AutoRefreshSpammer2));
                    loadedProfile.AutoRefreshSpammer3 = JsonConvert.DeserializeObject<AutoRefreshSpammer>(Profile.GetByAction(rawObject, loadedProfile.AutoRefreshSpammer3));
                    loadedProfile.Autobuff = JsonConvert.DeserializeObject<AutoBuff>(Profile.GetByAction(rawObject, loadedProfile.Autobuff));
                    loadedProfile.SongMacro = JsonConvert.DeserializeObject<Macro>(Profile.GetByAction(rawObject, loadedProfile.SongMacro));
                    loadedProfile.AtkDefMode = JsonConvert.DeserializeObject<ATKDEFMode>(Profile.GetByAction(rawObject, loadedProfile.AtkDefMode));
                    loadedProfile.MacroSwitch = JsonConvert.DeserializeObject<Macro>(Profile.GetByAction(rawObject, loadedProfile.MacroSwitch));
                    loadedProfile.DebuffsRecovery = JsonConvert.DeserializeObject<DebuffsRecovery>(Profile.GetByAction(rawObject, loadedProfile.DebuffsRecovery));
                    loadedProfile.PixelMacro = JsonConvert.DeserializeObject<PixelMacro>(Profile.GetByAction(rawObject, loadedProfile.PixelMacro));
                    loadedProfile.PixelMacro.EnsureRules();
                }

                return loadedProfile;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Profile] Error Message: {ex.Message}");
                throw new Exception("Houve um problema ao carregar o perfil. Delete a pasta Profiles e tente novamente.");
            }
        }

        public static void Use(Profile selectedProfile)
        {
            profile = selectedProfile;
        }

        public static void Create(string profileName)
        {
            string jsonFileName = AppConfig.ProfileFolder + profileName + ".json";

            if (!File.Exists(jsonFileName))
            {
                if (!Directory.Exists(AppConfig.ProfileFolder)) { Directory.CreateDirectory(AppConfig.ProfileFolder); }
                FileStream fs = File.Create(jsonFileName);
                fs.Close();

                Profile profile = new Profile(profileName);
                string output = JsonConvert.SerializeObject(profile, Formatting.Indented);
                File.WriteAllText(jsonFileName, output);
            }

            ProfileSingleton.Load(profileName);
        }

        public static void Delete(string profileName)
        {
            try
            {
                if (profileName != "Default") { File.Delete(AppConfig.ProfileFolder + profileName + ".json"); }
            }
            catch { }
        }

        public static void Rename(string oldProfileName, string newProfileName)
        {
            string jsonFileName = AppConfig.ProfileFolder + newProfileName + ".json";
            if (oldProfileName != "Default" && !File.Exists(jsonFileName)) {
                File.Move(AppConfig.ProfileFolder + oldProfileName + ".json", jsonFileName);
            }
        }

        public static void Copy(string profileName)
        {
            try
            {
                string jsonFileName = AppConfig.ProfileFolder + profileName + " Copy.json";
                if (profileName != "Default" && !File.Exists(jsonFileName)) {
                    File.Copy(AppConfig.ProfileFolder + profileName + ".json", jsonFileName);
                }
            }
            catch { }
        }

        public static void SetConfiguration(Action action)
        {
            if (profile != null)
            {
                string jsonData = File.ReadAllText(AppConfig.ProfileFolder + profile.Name + ".json");
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonData);
                jsonObj[action.GetActionName()] = action.GetConfiguration();
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(AppConfig.ProfileFolder + profile.Name + ".json", output);
            }
        }

        public static Profile GetCurrent()
        {
            return profile;
        }
    }

    public class Profile
    {
        public string Name { get; set; }
        public UserPreferences UserPreferences { get; set; }
        public AHK AHK { get; set; }
        public Autopot Autopot { get; set; }
        public Autopot AutopotYgg { get; set; }
        public AutoRefreshSpammer AutoRefreshSpammer1 { get; set; }
        public AutoRefreshSpammer AutoRefreshSpammer2 { get; set; }
        public AutoRefreshSpammer AutoRefreshSpammer3 { get; set; }
        public AutoBuff Autobuff { get; set; }
        public StatusRecovery StatusRecovery { get; set; }
        public Macro SongMacro { get; set; }
        public Macro MacroSwitch { get; set; }
        public PixelMacro PixelMacro { get; set; }

        public ATKDEFMode AtkDefMode { get; set; }
        public DebuffsRecovery DebuffsRecovery { get; set; }

        public Profile(string name)
        {
            this.Name = name;

            this.UserPreferences = new UserPreferences();
            this.AHK = new AHK();
            this.Autopot = new Autopot(Autopot.ACTION_NAME_AUTOPOT);
            this.AutopotYgg = new Autopot(Autopot.ACTION_NAME_AUTOPOT_YGG);
            this.AutoRefreshSpammer1 = new AutoRefreshSpammer(actionName: "AutoRefreshSpammer01");
            this.AutoRefreshSpammer2 = new AutoRefreshSpammer(actionName: "AutoRefreshSpammer02");
            this.AutoRefreshSpammer3 = new AutoRefreshSpammer(actionName: "AutoRefreshSpammer03");
            this.Autobuff = new AutoBuff();
            this.StatusRecovery = new StatusRecovery();
            this.SongMacro = new Macro(Macro.ACTION_NAME_SONG_MACRO, MacroSongForm.TOTAL_MACRO_LANES_FOR_SONGS);
            this.MacroSwitch = new Macro(Macro.ACTION_NAME_MACRO_SWITCH, MacroSwitchForm.TOTAL_MACRO_LANES);
            this.PixelMacro = new PixelMacro();
            this.AtkDefMode = new ATKDEFMode();
            this.DebuffsRecovery = new DebuffsRecovery();
        }

        public static object GetByAction(dynamic obj, Action action)
        {
            if (obj != null && obj[action.GetActionName()] != null)
            {
                return obj[action.GetActionName()].ToString();
            }

            return action.GetConfiguration();
        }

        public static List<string> ListAll()
        {
            List<string> profiles = new List<string>();
            try
            {
                string[] files = Directory.GetFiles(AppConfig.ProfileFolder);

                foreach (string fileName in files)
                {
                    string[] len = fileName.Split('\\');
                    string profileName = len[len.Length - 1].Split('.')[0];
                    profiles.Add(profileName);
                }
            }
            catch { }
            return profiles;
        }
    }

}
