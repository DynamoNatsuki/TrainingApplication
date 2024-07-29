using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TrainingApplication.Models;

namespace TrainingApplication.Controllers
{
    public class ExercisesController : Controller
    {

        public async Task<IActionResult> Index()
        {
            Database db = new Database();
            var exercises = await db.GetExercisesAsync();

            return View(exercises);
        }

        public async Task<IActionResult> Create()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            Database db = new Database();
            await db.SaveExercise(exercise);

            return Redirect("/Exercises");
        }

        public async Task<IActionResult> Details(ObjectId id)
        {
            Database db = new Database();
            var exercise = await db.GetExercise(id);

            return View(exercise);
        }
    }
}
