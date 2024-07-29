using MongoDB.Bson;

namespace TrainingApplication.Models
{
    public class Training
    {
        public ObjectId Id { get; set; }
        public ObjectId ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public DateOnly Date { get; set; }
        public double DistanceOrWeight { get; set; }
        public string Units { get; set; }
    }
}
