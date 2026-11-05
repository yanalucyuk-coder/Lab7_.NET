using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7.Classes
{
    public class Exam : Test
    {
        private double passingScore;

        public double PassingScore
        {
            get { return passingScore; }
            set
            {
                if (value >= 0 && value <= MaxScore()) { passingScore = value; }
                else { throw new Exception("Прохідний бал має бути від 0 до "+MaxScore()+"!"); }
            }
        }

        public Exam(string name, int questionCount,double time, double passingScore) : base(name, questionCount, time)
        {
            PassingScore = passingScore;
        }

        public override double MaxScore()
        {
            return QuestionCount*2;
        }

        public bool IsPassed(double score)
        {
            return score>=PassingScore;
        }
    }
}
