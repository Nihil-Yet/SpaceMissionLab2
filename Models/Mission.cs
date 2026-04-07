namespace SpaceMission.Models
{
    public enum MissionT
    {
        Unknow,
        Orbital,
        Planetary
    }

    public abstract class Mission
    {        
        public int Id { get; set; }

        protected string name;
        protected int budget;
        protected int duration;

        // геттеры/сеттеры
        public string Name 
        { 
            get => name; 
            set => name = value; 
        }

        public int Budget 
        { 
            get => budget; 
            set => budget = value; 
        }

        public int Duration 
        { 
            get => duration; 
            set => duration = value; 
        }

        public MissionT MissionType { get; set; }

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

        public virtual string GetInfo()
        {
            return $"Mission name: {name}\n" +
                   $"Budget: {budget} mln $\n" +
                   $"Duration: {duration} days\n" +
                   $"Budget per day: {BudgetPerDay()} mln $";
        }
    }
}
