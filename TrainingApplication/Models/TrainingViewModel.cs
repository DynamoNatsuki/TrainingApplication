namespace TrainingApplication.Models
{
    public class TrainingViewModel
    {
        public string Id { get; set; }
        public string ExerciseName { get; set; }
        public DateOnly Date { get; set; }
        public double DistanceOrWeight { get; set; }        
        public string Units { get; set; }

    }
}
