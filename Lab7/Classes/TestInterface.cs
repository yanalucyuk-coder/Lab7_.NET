using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7.Classes
{
    public class TestInterface : ITimeObserver, ILoggable
    {
        private bool isBlocked;

        public bool IsBlocked
        {
            get { return isBlocked; }
        }

        public string OnTimeExpired()
        {
            isBlocked = true;
            return "Час вийшов!";
        }

        public string OnFiveMinutesLeft()
        {
            return "Увага! Залишилося 5 хвилин!";
        }

        public void Unblock()
        {
            isBlocked = false;
        }

        public string Block()
        {
            isBlocked = true;                  // так само блокуємо кнопки
            return "Тест завершено!";          // але текст правдивий
        }

        public string GetStatus()
        {
            if (isBlocked) { return "Кнопки заблоковано!"; }
            else { return "Кнопки активні!"; }
        }

    }
}
