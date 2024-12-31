using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Humanizer;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace expensetracker.Controllers
{
    public class ExpenseTrackerController : Controller
    {
        private readonly expensedal expensedal;


        // Constructor to inject the data access layer
        public ExpenseTrackerController(expensedal dataAccess)
        {
            expensedal = dataAccess;
        }

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Aboutus()
        {
            return View();
        }
        public IActionResult Contactus()
        {
            return View();
        }



        public ActionResult Success()
        {
            return View();
        }



        // GET: /Account/SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public IActionResult SignIn(string Username, string Password)
        {
            var user = expensedal.Login(Username, Password);

            if (user != null)
            {
                // Validate required fields
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Role))
                {
                    TempData["Error"] = "User data is incomplete. Please contact support.";
                    return RedirectToAction("SignIn");
                }

                // Store user details in session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Password", user.Password);
                HttpContext.Session.SetString("UserRole", user.Role);

                TempData["Message"] = $"Welcome back, {user.Name}!";


                if (user.Role == "Admin")
                {
                    return RedirectToAction("adminhome", "admin");
                }
                else if (user.Role == "User")
                {
                    return RedirectToAction("userhome", "user");
                }
                else
                {
                    TempData["Error"] = "Invalid role.";
                    return RedirectToAction("SignIn");
                }
            }
            else
            {
                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction("SignIn");
            }
        }



        // Dashboard or homepage (accessible after login)

        // GET: Signup
        public IActionResult Create()
        {

            var model = new signup
            {
                States = GetStates(),
                Districts = new List<SelectListItem>()
            };
            return View(model);
        }

        // POST: Signup
        [HttpPost]
        public IActionResult Create(signup model)
        {
            if (!ModelState.IsValid)
            {
                // Save the user registration data
                bool isInserted = expensedal.InsertRegistration(model);

                if (isInserted)
                {
                    return RedirectToAction("Create");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while registering. Please try again.");
                }
            }

            model.States = GetStates();
            model.Districts = GetDistrictsByState(model.State);
            return View("Create", model);
        }

        // Handle dynamic loading of districts based on state

        [HttpPost]
        public JsonResult GetDistricts(string state)
        {
            var districts = GetDistrictsByState(state);

            // Ensure the data is in the format expected by the JavaScript
            var result = districts.Select(d => new { value = d.Value, text = d.Text }).ToList();
            return Json(result);
        }

        // Hardcoded method to get states (you can modify this to fetch from DB)
        private List<SelectListItem> GetStates()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "kerala", Text = "kerala" },
                new SelectListItem { Value = "tamilnadu", Text = "tamilnadu" },

            };
        }

        // Hardcoded method to get districts based on state
        private List<SelectListItem> GetDistrictsByState(string state)
        {
            var districts = new List<SelectListItem>();

            if (state == "kerala")
            {
                districts.Add(new SelectListItem { Value = "idukki", Text = "Idukki" });
                districts.Add(new SelectListItem { Value = "ernakulam", Text = "Ernakulam" });
            }
            else if (state == "tamilnadu")
            {
                districts.Add(new SelectListItem { Value = "madurai", Text = "Madurai" });
                districts.Add(new SelectListItem { Value = "salem", Text = "Salem" });
            }

            return districts;
        }

        // GET: View Signup by ID
        public IActionResult ViewSignup(int id)
        {
            var signup = expensedal.GetSignupById(id);
            if (signup == null)
            {
                return NotFound();
            }
            return View(signup);
        }
        // GET: Edit Signup
        public IActionResult Edit(int id)
        {
            var signup = expensedal.GetSignupById(id);
            if (signup == null)
            {
                return NotFound();
            }
            return View(signup);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, signup model)
        {
            // Corrected the comparison and type conversion
            if (id != int.Parse(model.Id))
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                bool isUpdated = expensedal.UpdateSignup(model);
                if (isUpdated)
                {
                    return RedirectToAction("Success");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the record.");
                }
            }
            return View(model);
        }
        // GET: Delete Signup
        public IActionResult Delete(int id)
        {
            var signup = expensedal.GetSignupById(id);
            if (signup == null)
            {
                return NotFound();
            }
            return View(signup);
        }

        // POST: Delete Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            bool isDeleted = expensedal.DeleteSignup(id);
            if (isDeleted)
            {
                return RedirectToAction("Success");
            }
            return RedirectToAction("Error");
        }

    }
}



