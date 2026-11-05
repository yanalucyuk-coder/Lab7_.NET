using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab7.Classes;

namespace Lab7.Tests
{
    internal static class Helper
    {
        public static bool Throws(Action action)
        {
            try
            {
                action();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }

    [TestClass]
    public class HierarchyTests
    {
        [TestMethod]
        public void TimePerQuestion_Correct()
        {
            Test t = new Test("Math", 20, 40);
            Assert.AreEqual(120.0, t.TimePerQuestion(), 0.0001);
        }

        [TestMethod]
        public void MaxScore_Test_EqualQuestionCount()
        {
            Test t = new Test("Math", 20, 40);
            Assert.AreEqual(20.0, t.MaxScore(), 0.0001);
        }

        [TestMethod]
        public void MaxScore_Exam_IsOverriden()
        {
            Exam e = new Exam("Exam", 20, 40, 24);
            Assert.AreEqual(40.0, e.MaxScore(), 0.0001);
        }

        [TestMethod]
        public void Exam_IsPassed()
        {
            Exam e = new Exam("Exam", 20, 40, 24);
            Assert.IsTrue(e.IsPassed(30));
            Assert.IsTrue(e.IsPassed(24));
            Assert.IsFalse(e.IsPassed(20));
        }

        [TestMethod]
        public void Exam_UsedAsTest()
        {
            Test t = new Exam("Exam", 20, 40, 24);
            Assert.IsTrue(t is Exam);
            Assert.AreEqual(40.0, t.MaxScore(), 0.0001);
        }

        [TestMethod]
        public void InvalidData_Exception()
        {
            Assert.IsTrue(Helper.Throws(() => new Test("", 10, 5)));
            Assert.IsTrue(Helper.Throws(() => new Test("   ", 10, 5)));
            Assert.IsTrue(Helper.Throws(() => new Test("T", 0, 5)));
            Assert.IsTrue(Helper.Throws(() => new Test("T", 10, 0)));
            Assert.IsTrue(Helper.Throws(() => new Test("T", 10, -1)));
            Assert.IsTrue(Helper.Throws(() => new Exam("E", 20, 40, 50)));
            Assert.IsTrue(Helper.Throws(() => new Exam("E", 20, 40, -1)));
            Assert.IsFalse(Helper.Throws(() => new Exam("E", 20, 40, 40)));
        }

    }

    [TestClass]
    public class TimerEventTests
    { 
        [TestMethod]
        public void Timer_ImplementsThreeInterfaces()
        {
            object timer = new TestTimer(new Test("T", 10, 2));
            Assert.IsTrue(timer is ITimerControl);
            Assert.IsTrue(timer is IResettable);
            Assert.IsTrue(timer is ILoggable);
        }

        [TestMethod]
        public void GetStatus_Initial()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 2));
            Assert.AreEqual("Залишилося: 02:00 (Тест зупинено)", timer.GetStatus());
        }

        [TestMethod]
        public void Start_Tick_DecreaseRemaining()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 2));
            timer.Start();
            timer.Tick();
            Assert.AreEqual(119, timer.RemainingSeconds);
            Assert.AreEqual("Залишилося: 01:59 (Тест триває)", timer.GetStatus());
        }

        [TestMethod]
        public void Tick_WithoutStart_ResultNothing()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 2));
            timer.Tick();
            Assert.AreEqual(120, timer.RemainingSeconds);
        }

        [TestMethod]
        public void Reset_RestoresTime()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 2));
            timer.Start();
            for(int i = 0; i < 10; i++) { timer.Tick(); }
            Assert.AreEqual(110, timer.RemainingSeconds);
            timer.Reset();
            Assert.AreEqual(120, timer.RemainingSeconds);
            Assert.IsFalse(timer.IsRunning);
        }

        [TestMethod]
        public void TimeExpired_FiresOne()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 1));
            int count = 0;
            timer.TimeExpired += () => count++;
            timer.Start();
            for (int i = 0; i < 60; i++) { timer.Tick(); }
            Assert.AreEqual(1, count);
            Assert.AreEqual(0, timer.RemainingSeconds);
            Assert.IsFalse(timer.IsRunning);
            for(int i = 0; i < 5; i++) { timer.Tick(); }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void FiveMinutesLeft_FiresAtWarningTime()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 1));
            timer.WarningSeconds = 10;
            int count = 0;
            timer.FiveMinutesLeft += () => count++;
            timer.Start();
            for (int i = 0; i < 49; i++) { timer.Tick(); }
            Assert.AreEqual(0, count);
            timer.Tick();
            Assert.AreEqual(1, count);
            Assert.AreEqual(10, timer.RemainingSeconds);
        }

        [TestMethod]
        public void TimeTick_PassesRemainingSeconds()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 1));
            int last = -1;
            timer.TimeTick += r => last = r;
            timer.Start();
            for (int i = 0; i < 3; i++) { timer.Tick(); }
            Assert.AreEqual(57, last);
        }

        [TestMethod]
        public void WarningSeconds_Invalid()
        {
            TestTimer timer = new TestTimer(new Test("T", 10, 1));
            Assert.IsTrue(Helper.Throws(() => timer.WarningSeconds = 0));
            Assert.IsTrue(Helper.Throws(() => timer.WarningSeconds = -5));
        }
    }

    [TestClass]
    public class ObserverTests
    {
        [TestMethod]
        public void TestInterface_OnTimeExpired_BlockButton()
        {
            TestInterface ui = new TestInterface();
            Assert.IsFalse(ui.IsBlocked);
            Assert.AreEqual("Кнопки активні!", ui.GetStatus());

            string text = ui.OnTimeExpired();
            Assert.AreEqual("Час вийшов!", text);
            Assert.IsTrue(ui.IsBlocked);
            Assert.AreEqual("Кнопки заблоковано!", ui.GetStatus());
        }

        [TestMethod]
        public void TestInterface_UnblockButton()
        {
            TestInterface ui = new TestInterface();
            ui.OnTimeExpired();
            ui.Unblock();
            Assert.IsFalse(ui.IsBlocked);
        }

        [TestMethod]
        public void Grading_Grade_UsesQuestionWeight()
        {
            GradingSystem simple = new GradingSystem(new Test("T", 10, 5));
            Assert.AreEqual(8.0, simple.Grade(8), 0.0001);

            GradingSystem exam = new GradingSystem(new Exam("E", 10, 5, 12));
            Assert.AreEqual(16.0, exam.Grade(8), 0.0001);
            Assert.AreEqual(0.0, exam.Grade(0), 0.0001);
            Assert.AreEqual(20.0, exam.Grade(10), 0.0001);
        }

        [TestMethod]
        public void Grading_SendForGrading_Passed_RaisesEvent()
        {
            GradingSystem g = new GradingSystem(new Exam("E", 10, 5, 12));
            g.CorrectAnswers = 8;

            string captured = "";
            int count = 0;
            g.GradingCompleted += r => { captured = r; count++; };

            string answer = g.SendForGrading();
            Assert.AreEqual("Роботу надіслано на перевірку!", answer);
            Assert.AreEqual("Результат: 16 з 20, іспит складено", captured);
            Assert.IsTrue(g.IsSent);

            Assert.AreEqual("Роботу вже надіслано!", g.SendForGrading());
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void Grading_SendForGrading_Failed()
        {
            GradingSystem g = new GradingSystem(new Exam("E", 10, 5, 12));
            g.CorrectAnswers = 5;

            string captured = "";
            g.GradingCompleted += r => captured = r;

            g.SendForGrading();
            Assert.AreEqual("Результат: 10 з 20, іспит не складено", captured);
        }

        [TestMethod]
        public void Grading_InvalidAnswers_Throws()
        {
            GradingSystem g = new GradingSystem(new Test("T", 10, 5));
            Assert.IsTrue(Helper.Throws(() => g.CorrectAnswers = 11));
            Assert.IsTrue(Helper.Throws(() => g.CorrectAnswers = -1));
            Assert.IsTrue(Helper.Throws(() => g.Grade(-1)));
        }
    }

    [TestClass]
    public class SessionTests
    {
        [TestMethod]
        public void Broadcast_EventsHaveTwoSubscribers()
        {
            TestSession s = new TestSession(new Test("T", 10, 1));
            Assert.AreEqual(2, s.Timer.TimeExpiredSubscribers);
            Assert.AreEqual(2, s.Timer.FiveMinutesLeftSubscribers);
        }

        [TestMethod]
        public void Broadcast_TimeExpired_CallsBothSubscribers()
        {
            TestSession s = new TestSession(new Test("T", 10, 1));
            s.Timer.Start();
            for (int i = 0; i < 60; i++) { s.Timer.Tick(); }

            Assert.IsTrue(s.Ui.IsBlocked);
            Assert.IsTrue(s.Grading.IsSent);
            Assert.AreEqual(3, s.Log.Count);
            Assert.AreEqual("[Інтерфейс] Час вийшов!", s.Log[0]);
            Assert.AreEqual("[Оцінювання] Результат: 0 з 10", s.Log[1]);
            Assert.AreEqual("[Оцінювання] Роботу надіслано на перевірку!", s.Log[2]);
        }

        [TestMethod]
        public void Broadcast_FiveMinutesLeft_CallsBothSubscribers()
        {
            TestSession s = new TestSession(new Test("T", 10, 1));
            s.Timer.WarningSeconds = 10;
            s.Timer.Start();
            for (int i = 0; i < 50; i++) { s.Timer.Tick(); }

            Assert.AreEqual(2, s.Log.Count);
            Assert.AreEqual("[Інтерфейс] Увага! Залишилося 5 хвилин!", s.Log[0]);
            Assert.AreEqual("[Журнал] Залишилося: 00:10 (Тест триває)", s.Log[1]);
        }

        [TestMethod]
        public void Multicast_Finish_RunsWholeChain()
        {
            TestSession s = new TestSession(new Exam("E", 10, 5, 12));
            s.Grading.CorrectAnswers = 8;
            Assert.AreEqual(4, s.ActionsCount);

            s.Finish();

            Assert.AreEqual(5, s.Log.Count);
            Assert.AreEqual("Таймер зупинено!", s.Log[0]);
            Assert.AreEqual("Тест завершено!", s.Log[1]);
            Assert.AreEqual("[Оцінювання] Результат: 16 з 20, іспит складено", s.Log[2]);
            Assert.AreEqual("Роботу надіслано на перевірку!", s.Log[3]);
            Assert.AreEqual("Підсумок: Залишилося: 05:00 (Тест зупинено); Кнопки заблоковано!", s.Log[4]);
            Assert.IsTrue(s.Ui.IsBlocked);
            Assert.IsTrue(s.Grading.IsSent);
        }

        [TestMethod]
        public void Multicast_RemoveAndAddAction()
        {
            TestSession s = new TestSession(new Exam("E", 10, 5, 12));
            s.Grading.CorrectAnswers = 8;

            s.RemoveAction(s.WriteSummary);
            Assert.AreEqual(3, s.ActionsCount);

            s.Finish();
            Assert.AreEqual(4, s.Log.Count);
            foreach (string line in s.Log) { Assert.IsFalse(line.StartsWith("Підсумок")); }

            s.AddAction(s.WriteSummary);
            Assert.AreEqual(4, s.ActionsCount);
        }

        [TestMethod]
        public void MessageLogged_RaisedForEveryLogEntry()
        {
            TestSession s = new TestSession(new Exam("E", 10, 5, 12));
            s.Grading.CorrectAnswers = 8;

            int count = 0;
            s.MessageLogged += m => count++;

            s.Finish();
            Assert.AreEqual(s.Log.Count, count);
            Assert.AreEqual(5, count);
        }
    }


}