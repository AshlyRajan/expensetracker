using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc;

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

        // GET: /ExpenseTracker/Register
        [HttpGet]
        [Route("ExpenseTracker/Register")]
        public IActionResult Register()
        {
            return View(new property());  // Return the registration form view
        }

        // POST: /ExpenseTracker/Register
        [HttpPost]
        [Route("ExpenseTracker/Register")]
        public IActionResult Register(property user)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                if (_dataAccess.IsEmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(user);  // Return the form with validation error
                }

                // Add the user to the database
                bool isUserAdded = _dataAccess.AddUser(user);
                if (isUserAdded)
                {
                    // Redirect to the success page
                    TempData["Message"] = "Registration successful!";
                    return RedirectToAction("RegistrationSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "There was an error registering the user.");
                    return View(user);  // Return the form with error message
                }
            }
            return View(user);  // If model is invalid, return the form with validation errors
        }

        // GET: /ExpenseTracker/RegistrationSuccess
        [HttpGet]
        [Route("ExpenseTracker/RegistrationSuccess")]
        public IActionResult RegistrationSuccess()
        {
            return View();  // Return the registration success view
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
    




//using expensetracker.DAL;
//using expensetracker.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace expensetracker.Controllers
//{
//    public class ExpenseTrackerController : Controller
//    {
//        [ApiController]
//        [Route("api/[controller]")]
//        public class ExpensesController : ControllerBase
//        {
//            private readonly expensedal _dataAccess;

//            public ExpensesController(expensedal dataAccess)
//            {
//                _dataAccess = dataAccess;
//            }



//            // GET: /ExpenseTracker/Register
//            [HttpGet]
//            [Route("ExpenseTracker/Register")]

//            public IActionResult Register()
//            {
//                return View();  // Return the registration view
//            }


//            // POST: /ExpenseTracker/Register
//            [HttpPost]
//            [Route("ExpenseTracker/Register")]
//            public IActionResult Register(property user)
//            {
//                if (ModelState.IsValid)
//                {
//                    // Check if email exists
//                    if (_dataAccess.IsEmailExists(user.Email))
//                    {
//                        ModelState.AddModelError("Email", "Email already exists.");
//                        return View(user);
//                    }

//                    // Add user to the database
//                    bool isUserAdded = _dataAccess.AddUser(user);
//                    if (isUserAdded)
//                    {
//                        return RedirectToAction("RegistrationSuccess");  // Redirect to success page
//                    }
//                    else
//                    {
//                        ModelState.AddModelError("", "There was an error registering the user.");
//                        return View(user);
//                    }
//                }
//                return View(user);  // If model is invalid, return the form with validation errors
//            }


//            // GET: /ExpenseTracker/RegistrationSuccess
//            [HttpGet]
//            [Route("ExpenseTracker/RegistrationSuccess")]
//            public IActionResult RegistrationSuccess()
//            {
//                return View();  // Return the registration success view
//            }
//        }
//    }
//}
