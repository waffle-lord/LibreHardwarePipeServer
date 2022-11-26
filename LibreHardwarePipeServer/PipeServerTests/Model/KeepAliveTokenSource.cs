namespace PipeServerTests.Model
{
    internal class KeepAliveTokenSource : IDisposable
    {
        private bool _isAlive = false;

        private bool _isTimeingOut = false;

        private CancellationTokenSource _cancelSource;

        private System.Timers.Timer _keepAliveTimer;

        public event EventHandler TokenCancelled;

        public bool IsCancelled => _cancelSource.IsCancellationRequested;

        public KeepAliveTokenSource(int timeout)
        {
            _cancelSource = new CancellationTokenSource();

            _keepAliveTimer = new System.Timers.Timer(timeout);

            _keepAliveTimer.Elapsed += _keepAliveTimer_Elapsed;

            _keepAliveTimer.Start();
        }

        public void KeepAlive()
        {
            _isAlive = true;
            _isTimeingOut = false;
        }

        public void Kill()
        {
            _keepAliveTimer.Stop();
            _cancelSource.Cancel();
            TokenCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void _keepAliveTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _keepAliveTimer.Stop();

            // continue waiting so long as KeepAlive is being called.
            if (_isAlive)
            {
                _isAlive = false;

                _keepAliveTimer.Start();

                return;
            }

            // allow one run after keep alive stops for possible connection issues
            if(!_isTimeingOut)
            {
                _isTimeingOut = true;

                _keepAliveTimer.Start();

                return;
            }

            // cancel token if source is no longer alive and is timeing out.
            Kill();
        }

        public void Dispose()
        {
            _keepAliveTimer.Dispose();
            _cancelSource.Dispose();
        }
    }
}
