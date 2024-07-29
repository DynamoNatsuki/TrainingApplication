/*Exercise description:
Create a training diary page using MVC. Users should be able to add exercises (e.g. deadlifting or running) and exercise sessions.
For each exercise, the user must be able to add the name and description of the exercise.
For each training session, the user must be able to add a date, choose an exercise and write how much they lifted or ran. (e.g. 2022-08-12, running, 5km).*/


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System.ComponentModel;
using System.Security.Cryptography;
using TrainingApplication.Models;

namespace TrainingApplication
{
    public class Database
    {
        MongoClient dbClient = new MongoClient();
        
        private IMongoDatabase GetDB() //Creates a database in MongoDB for this specific project. Can be used to connect other parts in the project to the database.
        {
            return dbClient.GetDatabase("TrainingsDB");
        }

        public IMongoCollection<Exercise> Exercises => GetDB().GetCollection<Exercise>("Exercises");
        public IMongoCollection<Training> Trainings => GetDB().GetCollection<Training>("Trainings");

        public async Task<List<Exercise>> GetExercisesAsync() //Create a list for the items (exercises) in the database and call the list Exercises. 
        {
            return await GetDB().GetCollection<Exercise>("Exercises")
                .Find(e => true)
                .ToListAsync();
        }

        public async Task SaveExercise(Exercise exercise) //With the information from the class Exercise gather the input given in the exercise variable and enter into the database.
        {
            await GetDB().GetCollection<Exercise>("Exercises")
                .InsertOneAsync(exercise);
        }

        public async Task<Exercise> GetExercise(ObjectId id) //Get the details of a specific exercise by filtering on the Id. Make sure that the Id is recognized as an ObjectId. Then create a variable to save the exercise which should be shown.
        {
            return await GetDB().GetCollection<Exercise>("Exercises")
                .Find(e => e.Id == id)
                .SingleOrDefaultAsync();    
        }

        public async Task<List<Training>> GetTrainingsAsync() //Create a list for the items (trainings) in the database and call the list Trainings. 
        {
            return await GetDB().GetCollection<Training>("Trainings")
                .Find(t => true)
                .ToListAsync();
        }

        public async Task SaveTraining(string exerciseId, DateOnly date, double distanceOrWeight, string units) //Save a new trainings session.
        {
            ObjectId _id = new ObjectId(exerciseId);

            var exercise = await GetDB().GetCollection<Exercise>("Exercises")
                .Find(e => e.Id == _id)
                .FirstOrDefaultAsync();

            if (exercise == null)
            {
                throw new Exception("Exercise not found");
            }

            var training = new Training
            {
                ExerciseId = exercise.Id,
                Date = date,
                DistanceOrWeight = distanceOrWeight,
                Units = units
            };

            await GetDB().GetCollection<Training>("Trainings")
                .InsertOneAsync(training);
        }

        public async Task<Training> ShowTraining(string id) //Show a specific training session.
        {
            ObjectId _id = new ObjectId(id);

            return await GetDB().GetCollection<Training>("Trainings")
                .Find(t => t.Id == _id)
                .SingleOrDefaultAsync();
        }

        public async Task EditTraining(string id, EditTrainingViewModel training) //Update the information on an existing training session.
        {
            ObjectId _id = new ObjectId(id);

            var filter = Builders<Training>.Filter.Eq(t => t.Id, _id);
            var update = Builders<Training>.Update
                .Set(t => t.ExerciseId, training.ExerciseId)
                //.Set(t => t.ExerciseName, training.ExerciseName)
                .Set(t => t.Date, training.Date)
                .Set(t => t.DistanceOrWeight, training.DistanceOrWeight)
                .Set(t => t.Units, training.Units);

            await GetDB().GetCollection<Training>("Trainings")
                .UpdateOneAsync(filter, update);
        }

        public async Task DeleteTraining(string id) //Delete a specific training session.
        {
            ObjectId _id = new ObjectId(id);

            await GetDB().GetCollection<Training>("Trainings")
                .DeleteOneAsync(t => t.Id == _id);
        }

        public static readonly List<string> ValidUnits = new List<string> { "m", "km", "kg" }; //Sets the property Units to a list of three specific items.

        public bool IsValidUnit(string unit) //Code that can be used when creating new or editing training sessions to check that no false values have been entered.
        {
            return ValidUnits.Contains(unit);
        }
    }
}
