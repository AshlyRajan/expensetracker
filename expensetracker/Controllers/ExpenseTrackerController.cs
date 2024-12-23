using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace expensetracker.Controllers
{
    public class ExpenseTrackerController : Controller
    {
        private readonly expensedal _dataAccess;
        

        // Constructor to inject the data access layer
        public ExpenseTrackerController(expensedal dataAccess)
        {
            _dataAccess = dataAccess;
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


        // Action for displaying the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Action for handling the form submission
        [HttpPost]
        public IActionResult Register(string Name, int Age, string Address, string Email, string Username, string Password)
        {
            try
            {
                // Call the InsertUser method of expensedal to insert data
                _dataAccess.InsertUser(Name, Age, Address, Email, Username, Password);

                // Set success message and redirect
                TempData["Message"] = "User registered successfully!";
                return RedirectToAction("Register");
            }
            catch (Exception ex)
            {
                // Handle errors and set failure message
                TempData["Error"] = $"Error adding user: {ex.Message}";
                return RedirectToAction("Register");
            }
        }
   


    // GET: /Account/SignIn
    [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public IActionResult SignIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Check user credentials in the database
                var user = _dataAccess.AuthenticateUser(model.Email, model.Password);
                if (user != null)
                {
                    // Store user information in the session (or use cookies/token as per your app)
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserName", user.Name);

                    // Redirect to dashboard or another page
                    return RedirectToAction("Dashboard", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }
    }
}






