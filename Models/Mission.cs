namespace SpaceMission
{
    public abstract class Mission
    {
        protected string name;
        protected int budget;
        protected int duration;

/*
        public string Name {
            get { return name; }
            set { name = value; }
        }
*/

        public Mission(string name, int budget, int duration)
        {
            this.name = name;
            this.budget = budget;
            this.duration = duration;
        }

        public int ExtendMission(int delta)
        {
            duration += delta;
            return duration;
        }

        public double BudgetPerDay()
        {
            return duration > 0 ? (double)budget / duration : 0.0;
        }

        public abstract double CalcFuelConsumption();
        public abstract float CalcRisk();

        // Virtual so derived classes can call base.GetInfo()
        public virtual string GetInfo()
        {
            return $"Mission name: {name}\n" +
                   $"Budget: {budget} mln $\n" +
                   $"Duration: {duration} days\n" +
                   $"Budget per day: {BudgetPerDay()} mln $";
        }
    }
}
