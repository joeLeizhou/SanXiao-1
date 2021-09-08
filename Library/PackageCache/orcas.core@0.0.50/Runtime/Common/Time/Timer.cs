using UnityEngine;
namespace Orcas
{
    public class Timer
    {
        public float Time => UnityEngine.Time.time - _startTime - _removeStopTime;

        public float RealTime => UnityEngine.Time.realtimeSinceStartup - _startRealTime - _removeStopRealTime;

        public bool Stopped => _running == false;

        private bool _running = false;
        private float _stopRealTime;
        private float _startRealTime;
        private float _stopTime;
        private float _startTime;
        private float _removeStopTime;
        private float _removeStopRealTime;

        public Timer(float startTime = 0)
        {
            Start(startTime);
        }

        public void Start(float startTime = 0)
        {
            _startTime = UnityEngine.Time.time - startTime;
            _startRealTime = UnityEngine.Time.realtimeSinceStartup - startTime;
            _removeStopTime = _removeStopRealTime = 0;
            _running = true;
        }

        public void Stop()
        {
            if (_running == false) return;
            _running = false;
            _stopTime = UnityEngine.Time.time;
            _stopRealTime = UnityEngine.Time.realtimeSinceStartup;
        }

        public void ReStart()
        {
            if (_running == true) return;
            _running = true;
            _removeStopTime += UnityEngine.Time.time - _stopTime;
            _removeStopRealTime += UnityEngine.Time.realtimeSinceStartup - _stopRealTime;
        }
    }
}