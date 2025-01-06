using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Humanizer;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver.Core.Configuration;

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
        // GET: ContactUs page
        [HttpGet]
        public IActionResult Contactus()
        {
            return View();
        }

        // POST: ContactUs page (Submit Form)
        [HttpPost]
        public IActionResult ContactUs(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                // Insert the contact message into the database using expensedal
                expensedal.InsertContactMessage(model);

                // Success message and redirect
                TempData["SuccessMessage"] = "Your message has been submitted successfully.";
                return RedirectToAction("ContactUs");
            }

            // If the model is not valid, return the view with the model so validation errors can be shown
            return View(model);
        }

        // GET: MessageIndex (Display all messages)
        [HttpGet]
        public IActionResult MessageIndex()
        {
            // Retrieve all contact messages from the database using expensedal
            var messages = expensedal.GetAllContactMessages();

            return View(messages);  // Pass the list of messages to the view
        }

        // POST: Delete a contact message
        [HttpPost]
        public IActionResult DeleteMessage(int messageId)
        {
            // Delete the contact message by its Id using expensedal
            expensedal.DeleteContactMessage(messageId);

            // Success message and redirect to MessageIndex
            TempData["SuccessMessage"] = "Message deleted successfully.";
            return RedirectToAction("MessageIndex");
        }
 
public ActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

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
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                

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
                TempData["Error"] = "Invalid email or password.";
                return RedirectToAction("SignIn");
            }
        }




        [HttpGet]
        public IActionResult Create()
        {
            var model = new signup
            {
                States = GetStates(),
                Districts = new List<SelectListItem>(), // Initially empty
                Role = "User" // Default role
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(signup model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    expensedal.InsertRegistration(model);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while registering: {ex.Message}");
                }
            }

            // Repopulate States and Districts for redisplay
            model.States = GetStates();
            model.Districts = GetDistrictsByState(model.State);
            return View(model);
        }

        private List<SelectListItem> GetStates()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "kerala", Text = "Kerala" },
                new SelectListItem { Value = "tamilnadu", Text = "Tamil Nadu" }
            };
        }

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
    


public IActionResult Profile()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId")); // Assuming UserId is stored in session
            var userDetails = expensedal.GetUserDetails(userId);

            if (userDetails == null)
            {
                TempData["Error"] = "User details not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(userDetails);
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
            try
            {
                expensedal.DeleteSignup(id); // Now a void method
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                // Handle the error if deletion fails
                ModelState.AddModelError("", $"An error occurred while deleting: {ex.Message}");
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public IActionResult Editsignup(int id)
        {
            var user = expensedal.GetSignupById(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Profile");
            }
            return View(user);
        }
        // POST: Editsignup
        [HttpPost]
        public IActionResult Editsignup(signup updatedUser)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    expensedal.UpdateSignup(updatedUser); // Now a void method
                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    // Handle any exception that occurs during the update
                    TempData["Error"] = $"An error occurred while updating the profile: {ex.Message}";
                }
            }

            // In case of validation failure or any exception, return the same view with the user data
            return View(updatedUser);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Contactus(ContactMessage model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            expensedal.InsertContactMessage(model); // Save the contact message
        //            TempData["Success"] = "Your message has been sent successfully.";
        //            return RedirectToAction("ContactUs");
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", $"An error occurred while sending your message: {ex.Message}");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Please correct the highlighted errors.");
        //    }

        //    return View(model); // Return the form with validation errors
        //}


    }
}



