using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7.Classes
{
    public class GradingSystem : ITimeObserver, IGradable
    {
        private Test test;
        private int correctAnswers;
        private bool isSent;

        public event Action<string> GradingCompleted;

        public bool IsSent
        {
            get { return isSent; }
        }

        public int CorrectAnswers
        {
            get { return correctAnswers; }
            set
            {
                if(value >=0 && value <= test.QuestionCount) { correctAnswers = value; }
                else { throw new Exception("К-сть правильних відповідей має бути від 0 до " + test.QuestionCount + "!"); }
            }
        }

        public GradingSystem(Test test)
        {
            if (test == null) { throw new Exception("Тест не заданий!"); }
            this.test = test;
        }

        public double Grade(int correctAnswers)
        {
            if (correctAnswers < 0 || correctAnswers > test.QuestionCount) { throw new Exception("К-сть правильних відповідей має бути від 0 до " + test.QuestionCount + "!"); }
            double pointsPerQuestion = test.MaxScore() / test.QuestionCount;
            return correctAnswers * pointsPerQuestion;
        }

        public string SendForGrading()
        {
            if (isSent) { return "Роботу вже надіслано!"; }
            isSent = true;
            double score = Grade(correctAnswers);
            string result = "Результат: " + score + " з " + test.MaxScore();

            if(test is Exam)
            {
                Exam exam = (Exam)test;
                if (exam.IsPassed(score)) { result += ", іспит складено"; }
                else { result += ", іспит не складено"; }
            }

            GradingCompleted?.Invoke(result);
            return "Роботу надіслано на перевірку!";
        }

        public string OnTimeExpired()
        {
            return SendForGrading();
        }

        public void Reset()
        {
            isSent = false;
            correctAnswers = 0;
        }


    }
}
