using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//7. (Тест, іспит...) → Система  Онлайн - тестування ;
// Сценарій: Студент проходить онлайн - тест з обмеженим часом.
// Тригер: Клас TestTimer(Таймер тесту).
// Подія 1 (широкомовна): TimeExpired(Час вийшов).
// Підписники:
//o TestInterface(Інтерфейс): Блокує кнопки, показує  Час вийшов!.
//o GradingSystem(Система): Автоматично надсилає роботу на перевірку.
// Подія 2: TestTimer.FiveMinutesLeft(Залишилось 5 хвилин).

namespace Lab7.Classes
{
    public delegate void TimerEventHandler();
    public class TestTimer : ITimerControl, IResettable, ILoggable
    {
        private Test test;
        private int remainingSeconds;
        private int warningSeconds = 300;
        private bool isRunning;

        public event TimerEventHandler TimeExpired;
        public event TimerEventHandler FiveMinutesLeft;
        public event Action<int> TimeTick;

        public int RemainingSeconds
        {
            get { return remainingSeconds; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public int TimeExpiredSubscribers
        {
            get
            {
                if(TimeExpired == null) { return 0; }
                return TimeExpired.GetInvocationList().Length;
            }
        }

        public int FiveMinutesLeftSubscribers
        {
            get
            {
                if (FiveMinutesLeft == null) { return 0; }
                return FiveMinutesLeft.GetInvocationList().Length;
            }
        }

        public int WarningSeconds
        {
            get { return warningSeconds; }
            set
            {
                if (value > 0) { warningSeconds = value; }
                else { throw new Exception("Час попередження має бути більшим за 0!"); }
            }
        }

        public TestTimer(Test test)
        {
            if(test == null) { throw new Exception("Тест не заданий"); }
            this.test = test;
            remainingSeconds = (int)Math.Round(test.Time * 60);
        }

        public void Start()
        {
            if (remainingSeconds > 0) { isRunning = true; }
        }

        public void Stop()
        {
            isRunning = false; 
        }

        public void Reset()
        {
            isRunning = false;
            remainingSeconds = (int)Math.Round(test.Time * 60);
        }

        public string GetStatus()
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            string state = isRunning ? "Тест триває" : "Тест зупинено";
            return "Залишилося: " + minutes.ToString("00") + ":" + seconds.ToString("00") + " (" + state + ")";
        }

        public void Tick()
        {
            if(!isRunning) { return; }
            remainingSeconds--;
            TimeTick?.Invoke(remainingSeconds);

            if(remainingSeconds == warningSeconds)
            {
                FiveMinutesLeft?.Invoke();
            }

            if(remainingSeconds <= 0)
            {
                remainingSeconds = 0;
                isRunning = false;
                TimeExpired?.Invoke();
            }
        }
    }
}
