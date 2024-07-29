using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Driver;
using TrainingApplication.Models;

namespace TrainingApplication.Controllers
{
    public class TrainingsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            Database db = new Database();

            var trainings = await db.GetTrainingsAsync();
            var exerciseIds = trainings.Select(t => t.ExerciseId).Distinct().ToList();

            var exercises = await db.GetExercisesAsync();
            var exerciseDict = exercises.ToDictionary(e => e.Id, e => e.ExerciseName);

            var trainingWithNames = trainings.Select(t => new TrainingViewModel
            {
                Id = t.Id.ToString(),
                ExerciseName = exerciseDict.ContainsKey(t.ExerciseId) ? exerciseDict[t.ExerciseId] : "Unknown",
                Date = t.Date,
                DistanceOrWeight = t.DistanceOrWeight,
                Units = t.Units
            }).ToList();

            return View(trainingWithNames);
        }

        public async Task<IActionResult> Details(string id)
        {
            Database db = new Database();

            var training = await db.ShowTraining(id);
            if (training == null)
            {
                return NotFound();
            }

            var exercise = await db.GetExercise(training.ExerciseId);
            var model = new TrainingViewModel
            {
                Id = training.Id.ToString(),
                ExerciseName = exercise?.ExerciseName ?? "Unknown",
                Date = training.Date,
                DistanceOrWeight = training.DistanceOrWeight,
                Units = training.Units
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            Database db = new Database();
            var exercises = await db.Exercises.Find(_ => true).ToListAsync();
            ViewBag.Exercises = new SelectList(exercises, "Id", "ExerciseName");
            ViewBag.ValidUnits = new SelectList(Database.ValidUnits);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainingViewModel model)
        {
            Database db = new Database();

            if (ModelState.IsValid)
            {
                var exercise = await db.GetExercise(model.ExerciseId);
                if (exercise == null)
                {
                    ModelState.AddModelError("", "Selected exercise does not exist.");
                    var exercises = await db.GetExercisesAsync();
                    ViewBag.Exercises = new SelectList(exercises, "Id", "ExerciseName");
                    ViewBag.ValidUnits = new SelectList(Database.ValidUnits);
                    return View(model);
                }

                var training = new Training
                {
                    ExerciseId = exercise.Id,
                    Date = model.Date,
                    DistanceOrWeight = model.DistanceOrWeight,
                    Units = model.Units
                };

                await db.SaveTraining(model.ExerciseId.ToString(), model.Date, model.DistanceOrWeight, model.Units);
                return RedirectToAction(nameof(Index));
            }

            var allExercises = await db.GetExercisesAsync();
            ViewBag.Exercises = new SelectList(allExercises, "Id", "ExerciseName");
            ViewBag.ValidUnits = new SelectList(Database.ValidUnits);

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            Database db = new Database();

            var training = await db.ShowTraining(id);
            if (training == null)
            {
                return NotFound();
            }

            var exercise = await db.GetExercise(training.ExerciseId);
            var model = new TrainingViewModel
            {
                Id = training.Id.ToString(),
                ExerciseName = exercise?.ExerciseName ?? "Unknown",
                Date = training.Date,
                DistanceOrWeight = training.DistanceOrWeight,
                Units = training.Units
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTraining(string id)
        {
            Database db = new Database();
            await db.DeleteTraining(id);

            return RedirectToAction(nameof(Index));  // Redirects to the Index action of the current controller. return RedirectToAction("Index", "Home") => Redirects to the Index action of the Home controller.
            //return Redirect("/Index"); => Redirects to the /Index path, which might not work as intended if routing or URL structure changes.
        }

        public async Task<IActionResult> Edit(string id)
        {
            Database db = new Database();

            // Retrieve the training to edit
            var training = await db.ShowTraining(id);
            if (training == null)
            {
                return NotFound(); // Or handle the case where the training does not exist
            }

            // Retrieve the list of exercises for the dropdown
            var exercises = await db.GetExercisesAsync();
            ViewBag.Exercises = new SelectList(exercises, "Id", "ExerciseName");
            ViewBag.ValidUnits = new SelectList(Database.ValidUnits);

            // Prepare the model for editing
            var model = new EditTrainingViewModel
            {
                Id = training.Id,
                ExerciseId = training.ExerciseId,
                Date = training.Date,
                DistanceOrWeight = training.DistanceOrWeight,
                Units = training.Units
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTrainingViewModel model)
        {
            Database db = new Database();

            if (ModelState.IsValid)
            {
                var exercise = await db.GetExercise(model.ExerciseId);

                if (exercise == null)
                {
                    ModelState.AddModelError("", "Selected exercise does not exist.");
                    ViewBag.Exercises = new SelectList(await db.GetExercisesAsync(), "Id", "ExerciseName");
                    ViewBag.ValidUnits = new SelectList(Database.ValidUnits);
                    return View(model);
                }

                var training = new EditTrainingViewModel
                {
                    ExerciseId = model.ExerciseId,
                    //ExerciseName = exercise.ExerciseName,
                    Date = model.Date,
                    DistanceOrWeight = model.DistanceOrWeight,
                    Units = model.Units
                };

                await db.EditTraining(model.Id.ToString(), training);

                return RedirectToAction(nameof(Index));
            }

            // Re-populate ViewBag if the model state is invalid
            ViewBag.Exercises = new SelectList(await db.GetExercisesAsync(), "Id", "ExerciseName");
            ViewBag.ValidUnits = new SelectList(Database.ValidUnits);

            return View(model);
        }
    }
}
