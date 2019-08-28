using System;
using System.Windows.Controls;
using Triton.Bot;
using Triton.Common;
using Triton.Common.LogUtilities;
using Triton.Game;
using log4net;
using SilverFish.ai;

namespace AutoConcede
{
    public class AutoConcede : IPlugin
    {
        private static readonly ILog Log = Hearthbuddy.Windows.MainWindow.ChuckLog;

        private int _defeatCount;

        private int _winCount = 1;

        private double _winRateThreshold = 0.2;

        private double _realWinRate = 1;

        private bool _resetWinCount = false;

        public void Start()
        {
            Log.DebugFormat("[AutoConcede] Initialize");
            CustomEventManager.Instance.MulliganStarted += CustomEventManager_MulliganStarted;
            GameEventManager.GameOver += GameEventManager_GameOver;
        }

        private void CustomEventManager_MulliganStarted(object sender, MulliganStartedEventArgs e)
        {
            if (_realWinRate >= _winRateThreshold)
            {
                e.ConcedeSuccessfully = TritonHs.Concede(true);
            }
            else
            {
                if (!_resetWinCount)
                {
                    _winCount--;
                    _resetWinCount = true;
                }
            }
        }

        private void GameEventManager_GameOver(object sender, GameOverEventArgs e)
        {
            if (e.Result == GameOverFlag.Victory)
            {
                _winCount++;

            }
            else if (e.Result == GameOverFlag.Defeat)
            {
                _defeatCount++;
            }

            CalculateRealWinRate();
        }

        private void CalculateRealWinRate()
        {
            double totalCount;
            if (!_resetWinCount)
            {
                totalCount = _winCount + _defeatCount - 1;
            }
            else
            {
                totalCount = _winCount + _defeatCount;
            }
            _realWinRate = _winCount / totalCount;
			
			Log.InfoFormat($"[AutoConcede]  _winCount = {_winCount}");
			Log.InfoFormat($"[AutoConcede]  _defeatCount = {_defeatCount}");
			Log.InfoFormat($"[AutoConcede]  totalCount = {totalCount}");
            Log.InfoFormat($"[AutoConcede] CalculateRealWinRate _realWinRate = {_realWinRate}");
        }

        public void Tick()
        {
        }

        public void Stop()
        {
            CustomEventManager.Instance.MulliganStarted -= CustomEventManager_MulliganStarted;
            GameEventManager.GameOver -= GameEventManager_GameOver;
        }

        public string Name => "AutoConcede";

        public string Author => "Chuck Lu";

        public string Description => "Auto concede to reduce the odds.";

        public string Version => "0.1.2";

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
        }

        public UserControl Control => null;

        public JsonSettings Settings => null;

        public void Enable()
        {
			IsEnabled = true;
			Log.DebugFormat("[AutoConcede] Enabled");
        }

        public void Disable()
        {
			IsEnabled = false;
			Log.DebugFormat("[AutoConcede] Disabled");
        }

        public bool IsEnabled { get; private set; }
    }
}
