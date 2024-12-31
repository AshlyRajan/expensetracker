using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using expensetracker.Models;
using expensetracker.DAL;

namespace expensetracker.Controllers
{
    public class AdminController : Controller
    {
        private readonly adminDAL adminDAL;

        public AdminController(adminDAL expenseDAL)
        {
            adminDAL = expenseDAL;
        }
        
            // GET: adminController
            public ActionResult adminhome()
            {
                var expenses = adminDAL.GetAllExpenses();
                return View(expenses);
                
            }
        public IActionResult CreateExpense()
        {
            return View();
        }


        public IActionResult AddExpenseType()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddExpenseType(string ExpenseTypes)
        {
            if (ModelState.IsValid)
            {
                // Add the new expense type to the database
                adminDAL.AddExpenseType(ExpenseTypes);

                // Redirect to the ListExpenseTypes page
                return RedirectToAction("ListExpenseTypes");
            }

            // If validation fails, redisplay the form
            return View(ExpenseTypes);
        }
      


    public IActionResult ListExpenseTypes()
        {
            var expenseTypes = adminDAL.GetAllExpenseTypes(); // Fetch all expense types
            return View(expenseTypes); // Pass the data to the view
        }

        public IActionResult EditExpenseType(int id)
        {
            var expenseType = adminDAL.GetExpenseTypeById(id);
            if (expenseType == null)
            {
                return NotFound();
            }
            return View(expenseType);
        }

        [HttpPost]
        public IActionResult EditExpenseType(int id, string ExpenseType)
        {
            if (ModelState.IsValid)
            {
                adminDAL.UpdateExpenseType(id, ExpenseType);
                TempData["SuccessMessage"] = "Expense Type updated successfully.";
                return RedirectToAction("ListExpenseTypes");
            }
            return View(ExpenseType);
        }


        public IActionResult DeleteExpenseType(int id)
        {
            var expenseType = adminDAL.GetExpenseTypeById(id);
            if (expenseType == null)
            {
                return NotFound();
            }
            return View(expenseType);
        }

        [HttpPost]
        public IActionResult ConfirmDeleteExpenseType(int id)
        {
            adminDAL.DeleteExpenseType(id);
            return RedirectToAction("ListExpenseTypes");
        }






        // Edit
        [HttpGet]
        public IActionResult Editsignup(int id)
        {
            var signup = adminDAL.GetSignupById(id);
            if (signup == null)
            {
                return NotFound();
            }
            return View(signup);
        }

        [HttpPost]
        public IActionResult Editsignup(signup signup)
        {
            if (ModelState.IsValid)
            {
                adminDAL.UpdateSignup(signup);
                return RedirectToAction("Indexadmin");
            }
            return View(signup);
        }

        // Delete
        [HttpGet]
        public IActionResult Deletesignup(int id)
        {
            var signup = adminDAL.GetSignupById(id);
            if (signup == null)
            {
                return NotFound();
            }
            return View(signup);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            adminDAL.DeleteSignup(id);
            return RedirectToAction("Indexadmin");
        }

        // List
        public IActionResult Indexadmin()
        {
            // Assume GetAllSignups is implemented in DAL to fetch all records.
            var signups = adminDAL.GetAllSignups();
            return View(signups);
        }
    

    // Logout
    public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("SignIn", "expensetracker");
        }

        [HttpPost]
        public ActionResult HandleAction(int expenseId, string action, int? approvedAmount = null)
        {
            try
            {
                if (action == "accept")
                {
                    adminDAL.UpdateExpenseStatus(expenseId, "Approved", approvedAmount);
                    return RedirectToAction("EnterApprovedAmount", new { expenseId });
                }
                else if (action == "decline")
                {
                    adminDAL.UpdateExpenseStatus(expenseId, "Rejected");
                    return RedirectToAction("adminhome");
                }
            }
            catch (Exception ex)
            {
                // Log exception (e.g., to a file or database)
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            }
            return RedirectToAction("adminhome");
        }


        public ActionResult EnterApprovedAmount(int expenseId)
        {
            ViewBag.ExpenseId = expenseId;
            return View();
        }

        [HttpPost]
        public ActionResult SaveApprovedAmount(int expenseId, int approvedAmount)
        {
            adminDAL.UpdateExpenseStatus(expenseId, "Approved", approvedAmount);
            return RedirectToAction("adminhome");
        }


        // GET: Admin Page to View Expenses
        public IActionResult joineddetails()
        {
            // Get the joined expense data
            var expenses = adminDAL.GetExpensesWithApprovedAmount();

            return View(expenses);  // Pass the data to the view
        }
    }
}

    



    // Manage Employees
    //public IActionResult ManageEmployees()
    //{
    //    var employees = adminDAL.GetAllEmployees();
    //    return View(employees);
    //}

    //public IActionResult CreateEmployee()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public IActionResult CreateEmployee(Registration employee)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        adminDAL.AddEmployee(employee);
    //        return RedirectToAction("ManageEmployees");
    //    }
    //    return View(employee);
    //}

    //public IActionResult EditEmployee(int id)
    //{
    //    var employee = adminDAL.GetEmployeeById(id);
    //    if (employee == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(employee);
    //}

    //[HttpPost]
    //public IActionResult EditEmployee(int id, Registration employee)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        adminDAL.UpdateEmployee(id, employee);
    //        return RedirectToAction("ManageEmployees");
    //    }
    //    return View(employee);
    //}

    //public IActionResult DeleteEmployee(int id)
    //{
    //    var employee = adminDAL.GetEmployeeById(id);
    //    if (employee == null)
    //    {
    //        return NotFound();
    //    }
    //    return View(employee);
    //}

    //[HttpPost, ActionName("DeleteEmployee")]
    //public IActionResult ConfirmDeleteEmployee(int id)
    //{
    //    adminDAL.DeleteEmployee(id);
    //    return RedirectToAction("ManageEmployees");
    //}



