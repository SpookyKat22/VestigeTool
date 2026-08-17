using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Forms;
using Newtonsoft.Json;
using _4RTools.Utils;

namespace _4RTools.Model
{
    public class AutoRefreshSpammer : Action
    {
        private _4RThread thread;
        public string ActionName { get; set; }
        public int RefreshDelay { get; set; } = 1000;
        public Key RefreshKey { get; set; }

        public AutoRefreshSpammer(string actionName)
        {
            this.ActionName = actionName;
        }

        public void Start()
        {
            Client roClient = ClientSingleton.GetClient();
            if (roClient != null)
            {
                const int defaultDelayInMilliseconds = 1000;
                int delay = this.RefreshDelay == 0 ? defaultDelayInMilliseconds : this.RefreshDelay;
                this.thread = new _4RThread(_ => AutorefreshThreadExecution(roClient, delay));
                _4RThread.Start(this.thread);
            }
        }

        private int AutorefreshThreadExecution(Client roClient, int delay)
        {
            if (roClient.IsGameActive() && this.RefreshKey != Key.None)
            {
                Interop.PostMessage(roClient.process.MainWindowHandle, Constants.WM_KEYDOWN_MSG_ID, (Keys)Enum.Parse(typeof(Keys), this.RefreshKey.ToString()), 0);
            }
            Thread.Sleep(delay);
            return 0;
        }

        public void Stop()
        {
            _4RThread.Stop(this.thread);
        }

        public string GetConfiguration()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string GetActionName()
        {
            return this.ActionName;
        }
    }
}
