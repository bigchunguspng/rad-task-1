using System;
using System.Timers;
using Microsoft.Win32;

namespace RadencyTaskETL
{
    public class MidnightTimer
    {
        private readonly Timer _timer;
        
        public MidnightTimer()
        {
            _timer = new Timer(GetSleepTime());
            _timer.Elapsed += (sender, args) =>
            {
                DayChanged?.Invoke(this, EventArgs.Empty);
                _timer.Interval = GetSleepTime();
            };
            _timer.Start();

            SystemEvents.TimeChanged += OnSystemTimeChanged;
        }

        public bool Enabled
        {
            get => _timer.Enabled;
            set => _timer.Enabled = value;
        }

        private double GetSleepTime()
        {
            var midnight = DateTime.Today.AddDays(1);
            return (midnight - DateTime.Now).TotalMilliseconds;
        }
        
        private void OnSystemTimeChanged(object sender, EventArgs e) => _timer.Interval = GetSleepTime();

        public event EventHandler<EventArgs> DayChanged;
    }
}