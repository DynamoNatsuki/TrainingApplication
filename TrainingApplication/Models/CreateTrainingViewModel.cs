using MongoDB.Bson;

namespace TrainingApplication.Models
{
    public class CreateTrainingViewModel
    {
        public ObjectId ExerciseId { get; set; }
        public DateOnly Date { get; set; }
        public double DistanceOrWeight { get; set; }
        public string Units { get; set; }

    }
}