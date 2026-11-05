using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Lab7.Classes;

namespace Lab7
{
    public partial class MainWindow : Window
    {
        private TestSession session;
        private DispatcherTimer clock;
        public MainWindow()
        {
            InitializeComponent();

            clock = new DispatcherTimer();
            clock.Interval = TimeSpan.FromSeconds(1);
            clock.Tick += Clock_Tick;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if(!double.TryParse(txtMinutes.Text, out double minutes) || minutes <=0)
            {
                LogMessage("Помилка: введіть коректну тривалість у хв!");
                return;
            }

            if (!int.TryParse(txtAnswers.Text, out int answers))
            {
                LogMessage("Помилка: введіть коректну к-сть правильних відповідей!");
                return;
            }

            try
            {
                logListBox.Items.Clear();

                Exam exam = new Exam("Онлайн-тест", 10, minutes,12);
                session = new TestSession(exam);
                session.Grading.CorrectAnswers = answers;

                session.MessageLogged += LogMessage;
                session.Timer.TimeTick += OnTick;
                session.Timer.TimeExpired += OnTimeExpired;

                session.Timer.Start();
                clock.Start();
                SetRunning(true);
                LogMessage("Тест розпочато. " + session.Timer.GetStatus());
            }
            catch (Exception ex)
            {
                LogMessage("Помилка: " + ex.Message);
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(txtAnswers.Text, out int answers))
            {
                LogMessage("Помилка: введіть коректну к-сть правильних відповідей");
                return;
            }

            try
            {
                session.Grading.CorrectAnswers = answers;
            }
            catch (Exception ex)
            {
                LogMessage("Помилка: " +ex.Message);
                return;
            }

            session.Finish();
            clock.Stop();
            SetRunning(false);
        }

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            ApplyAnswers();
            for(int i = 0; i < 60 && session.Timer.IsRunning; i++)
            {
                session.Timer.Tick();
            }
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            ApplyAnswers();
            session.Timer.Tick();
        }

        private void OnTick(int remainingSeconds)
        {
            lblTime.Text = "Залишилося: " + (remainingSeconds / 60).ToString("00") + ":" + (remainingSeconds % 60).ToString("00");
        }

        private void OnTimeExpired()
        {
            clock.Stop();
            SetRunning(false);
        }

        private void LogMessage(string message)
        {
            logListBox.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message);
            logListBox.ScrollIntoView(logListBox.Items[logListBox.Items.Count-1]);
        }

        private void ApplyAnswers()
        {
            if(int.TryParse(txtAnswers.Text, out int answers))
            {
                try
                {
                    session.Grading.CorrectAnswers = answers;
                }
                catch { }
            }
        }

        private void SetRunning(bool running)
        {
            btnStart.IsEnabled = !running;
            btnFinish.IsEnabled = running;
            btnSpeed.IsEnabled = running;
            txtMinutes.IsEnabled = !running;
        }
    }
}