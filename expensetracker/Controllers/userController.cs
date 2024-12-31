using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using expensetracker.Models;
using expensetracker.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public ActionResult userhome()
        {
            var expenses = duserDAL.GetAllExpenses();

            if (expenses == null)
            {
                expenses = new List<Expenz>(); // Initialize an empty list if null
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
        public IActionResult ViewExpenses()
        {
            // Get all expenses from the database
            List<Expenz> expenses = duserDAL.GetAllExpenses();

            // Pass the expenses to the view
            return View(expenses);
        }

        public IActionResult manage()
        {
            // Fetch the list of expenses from the database
            List<Expenz> expenses = duserDAL.GetAllExpenses();

            // Return the view with the list of expenses
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

                bool updated = duserDAL.UpdateExpenz(expense.Expenseid, expense.ExpenseTypeID, expense.Amount, expense.DateofExpense, expense.Description, billCopyPath, expense.Status);
                if (updated)
                {
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                    return RedirectToAction(nameof(manage));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the expense.");
                }
            }
            return View(expense);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var success = duserDAL.DeleteExpenz(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Expense deleted successfully!";
                return RedirectToAction(nameof(manage));
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the expense.";
                return RedirectToAction(nameof(manage));
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("SignIn", "expensetracker");
        }
    }
}


        