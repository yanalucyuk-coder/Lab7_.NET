using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7.Classes
{
    public delegate void SessionAction();
    public class TestSession
    {
        private Test test;
        private TestTimer timer;
        private GradingSystem grading;
        private TestInterface ui;
        private List<string> log = new List<string>();

        public SessionAction FinishActions;

        public event Action<string> MessageLogged;

        public Test Test { get { return test; } }
        public TestTimer Timer { get { return timer; } }
        public GradingSystem Grading { get { return grading; } }
        public TestInterface Ui { get { return ui; } }
        public List<string> Log { get { return log; } }

        public int ActionsCount
        {
            get
            {
                if (FinishActions == null) { return 0; }
                return FinishActions.GetInvocationList().Length;
            }
        }

        public TestSession(Test test)
        {
            if(test == null) { throw new Exception("Тест не заданий!");}
            this.test = test;
            timer = new TestTimer(test);
            grading = new GradingSystem(test);
            ui = new TestInterface();

            //побудова багатоадресного делегату
            FinishActions = StopTimer;
            FinishActions += LockInterface;
            FinishActions += SendWork;
            FinishActions += WriteSummary;

            //підписка на широкомовну подію TimeExpired
            timer.TimeExpired += OnExpiredInterface;
            timer.TimeExpired += OnExpiredGrading;

            timer.FiveMinutesLeft += OnWarningInterface;
            timer.FiveMinutesLeft += OnWarningLog;

            grading.GradingCompleted += OnGradingCompleted;
        }

        private void Write(string message)
        {
            log.Add(message);
            MessageLogged?.Invoke(message);
        }

        private void OnExpiredInterface()
        {
            //час вийшов
            Write("[Інтерфейс] " + ui.OnTimeExpired());
        }

        private void OnExpiredGrading()
        {
            //автоматичне надсилання роботи на первевірку
            Write("[Оцінювання] " + grading.OnTimeExpired());
        }

        private void OnWarningInterface()
        {
            Write("[Інтерфейс] " + ui.OnFiveMinutesLeft());
        }

        private void OnWarningLog()
        {
            Write("[Журнал] " + timer.GetStatus());
        }

        private void OnGradingCompleted(string result)
        {
            Write("[Оцінювання] " + result);
        }

        public void StopTimer()
        {
            timer.Stop();
            Write("Таймер зупинено!");
        }

        public void LockInterface()
        {
            Write(ui.Block());
        }

        public void SendWork()
        {
            Write(grading.SendForGrading());
        }

        public void WriteSummary()
        {
            Write("Підсумок: " + timer.GetStatus() + "; " + ui.GetStatus());
        }

        public void AddAction(SessionAction action)
        {
            FinishActions += action;
        }

        public void RemoveAction(SessionAction action)
        {
            FinishActions -= action;
        }

        public void Finish()
        {
            if (FinishActions != null) { FinishActions(); }
        }

    }
}
