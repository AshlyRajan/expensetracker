using System.ComponentModel.DataAnnotations;

namespace expensetracker.Models
{
    public class property
    {
         

            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Full Name is required")]
            
            public string Name { get; set; }
             [Required]
           public string Age { get; set; }
           [Required]
            public string Address { get; set; }

           [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            public string Email { get; set; }
            //public string Date { get; set; }
            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; }
             public string Role { get; set; }
             //public DateTime CreatedAt { get; set; } = DateTime.Now;
             //public int UserId { get; set; }
           
    }
    }

      public class LoginModel
       {
      [Required(ErrorMessage = "Email is required.")]
      [EmailAddress(ErrorMessage = "Invalid email address.")]
       public string Email { get; set; }

       [Required(ErrorMessage = "Password is required.")]
       [DataType(DataType.Password)]
       public string Password { get; set; }
       }

public class userproperty
       {  
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
   
        }

        //public class Expense
        //{
        //    public int Id { get; set; }
        //    public decimal Amount { get; set; }
        //    public int CategoryId { get; set; }
        //    public string CategoryName { get; set; }
        //    public DateTime Date { get; set; }
        //    public string Description { get; set; }
        //    public int UserId { get; set; }
        //    public DateTime CreatedDate { get; set; }
        //}

        //public class Income
        //{
        //    public int Id { get; set; }
        //    public decimal Amount { get; set; }
        //    public string Source { get; set; }
        //    public DateTime Date { get; set; }
        //    public string Description { get; set; }
        //    public int UserId { get; set; }
        //    public DateTime CreatedDate { get; set; }
        //}
    

