using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;

namespace user.Controllers
{
    public class userController : Controller
    {
        private readonly UserDAL duserDAL; // Use a private field with a distinct name to avoid conflicts.

        public userController(UserDAL userDAL)
        {
            duserDAL = userDAL ?? throw new ArgumentNullException(nameof(userDAL)); // Assign the injected instance to the private field.
        }

        // GET: userController
        [HttpGet]
        public async Task<IActionResult> userhome()
        {
            // Retrieve the logged-in username from the session
            string loggedInUser = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(loggedInUser))
            {
                TempData["Error"] = "You need to log in first.";
                return RedirectToAction("SignIn", "User");
            }
            ViewBag.Username = loggedInUser;
            // Get the list of expenses for the logged-in user
            var expenses = await duserDAL.GetUserExpensesAsync(loggedInUser);

            return View(expenses);

        }
       
    

    

    public IActionResult ViewExpenses()
        {
            // Get all expenses from the database
            List<Expenz> expenses = duserDAL.GetAllExpenses();

            // Pass the expenses to the view
            return View(expenses);
        }

       

        public IActionResult manage()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Please log in to view your expenses.";
                return RedirectToAction("SignIn", "Account");
            }

            var expenses = duserDAL.GetExpensesByUserby(username);
            return View(expenses);
        }
    


    // GET: UserController/Details/5
    [HttpGet]
        public IActionResult details(int id)
        {
            var expense = duserDAL.GetAllExpenses().FirstOrDefault(e => e.Expenseid == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: UserController/Update/5
        public IActionResult Update(int id)
        {
            var expense = duserDAL.GetAllExpenses().FirstOrDefault(e => e.Expenseid == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Expenz expense, IFormFile BillCopy)
        {
            if (ModelState.IsValid)
            {
                string billCopyPath = expense.BillCopy; // Preserve the original BillCopy if no new file is uploaded

                if (BillCopy != null && BillCopy.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BillCopy.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await BillCopy.CopyToAsync(stream);
                    }
                    billCopyPath = "/uploads/" + fileName;
                }

                try
                {
                    // Now calling the method with void return type, so no boolean check
                    duserDAL.UpdateExpenz(expense.Expenseid, expense.ExpenseTypeID, expense.Amount, expense.DateofExpense, expense.Description, billCopyPath, expense.Status);
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                    return RedirectToAction(nameof(manage));
                }
                catch (Exception ex)
                {
                    // If an exception occurs, display the error message
                    ModelState.AddModelError("", $"An error occurred while updating the expense: {ex.Message}");
                }
            }

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                duserDAL.DeleteExpenz(id); // Call DAL method to delete expense
                TempData["SuccessMessage"] = "Expense deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the expense: {ex.Message}";
            }

            return RedirectToAction(nameof(manage)); // Redirect back to the expenses list page
        }








        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Delete(int id)
        //{
        //    try
        //    {
        //        // Calling the void method to delete the expense
        //        duserDAL.DeleteExpenz(id);
        //        TempData["SuccessMessage"] = "Expense deleted successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        // If an exception occurs during deletion, display the error message
        //        TempData["ErrorMessage"] = $"An error occurred while deleting the expense: {ex.Message}";
        //    }

        //    return RedirectToAction(nameof(manage));
        //}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("SignIn", "expensetracker");
        }
        public IActionResult UserExpenses()
        {
            string createdBy = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(createdBy))
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("SignIn", "Account");
            }

            var expenses = duserDAL.GetExpensesByUser(createdBy);

            if (expenses == null || !expenses.Any())
            {
                TempData["Info"] = "No expenses found.";
                return View(new List<Expenz>());
            }

            return View(expenses);
        }
        [HttpGet]
        public IActionResult Create()
        {

            var expenseTypes = duserDAL.GetExpenseTypes();

            // Assign to ViewBag
            ViewBag.ExpenseTypes = expenseTypes ?? new List<SelectListItem>();

            return View(new Expenz());

        }

        [HttpPost]
        public async Task<IActionResult> Create(Expenz model, IFormFile? BillCopy)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    // Handle file upload only if a file is selected
                    if (BillCopy != null && BillCopy.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BillCopy.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await BillCopy.CopyToAsync(stream);
                        }

                        model.BillCopy = "/uploads/" + fileName; // Save relative path
                    }
                    else
                    {
                        // If no file is uploaded, set BillCopy as null or handle it as needed
                        model.BillCopy = null;
                    }

                    // Save the data to the database
                    await duserDAL.InsertExpenzAsync(
                        model.ExpenseTypeID,
                        model.Amount,
                        model.DateofExpense,
                        model.Description,
                        model.CreatedBy,
                        model.CreatedAt,
                        model.BillCopy ?? string.Empty, // Handle null gracefully
                        model.Status
                    );

                    TempData["SuccessMessage"] = "Expense created successfully!";
                    return RedirectToAction("userhome");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // Reload the expense types if validation fails
            ViewBag.ExpenseTypes = duserDAL.GetExpenseTypes();
            return View(model);
        }
    }
}


        