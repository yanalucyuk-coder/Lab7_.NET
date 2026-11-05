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
    public class Test
    {
        private string name;
        private double time;
        private int questionCount;

        public string Name
        {
            get { return name; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value)) { name = value; }
                else { throw new Exception("Назва тесту не може бути порожньою!"); }
            }
        }
        public double Time
        {
            get { return time; }
            set
            {
                if (value > 0) { time = value; }
                else { throw new Exception("Тривалість тесту має бути більщою за 0!"); }
            }
        }

        public int QuestionCount
        {
            get { return questionCount; }
            set
            {
                if (value > 0) { questionCount = value; }
                else { throw new Exception("Кількість питань має бути більщою за 0!"); }
            }
        }

        public Test(string name, int questionCount, double time)
        {
            Name = name;
            QuestionCount = questionCount;
            Time = time;
        }

        public double TimePerQuestion()
        {
            return Time*60/ QuestionCount;
        }

        public virtual double MaxScore()
        {
            return QuestionCount;
        }
    }
}
