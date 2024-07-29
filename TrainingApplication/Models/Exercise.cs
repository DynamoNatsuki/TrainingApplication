using MongoDB.Bson;

namespace TrainingApplication.Models
{
    public class Exercise
    {
        public ObjectId Id { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
    }
}
