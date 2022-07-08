using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserManagementApp.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace UserManagementApp.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Register model)
        {
            using (var context = new UsersManagementEntities())
            {
                model.Password = Encrypt(model.Password);
                bool isValid = context.User.Any(x => x.Username == model.Username && x.Password == model.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Create", "UploadData");
                }
            }
            ModelState.AddModelError("", "Invalid Username and Password");
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        //Save sign up done by user with redirect to login page
        [HttpPost]
        public ActionResult Signup(User model)
        {
            model.Password= Encrypt(model.Password);

            using (var context = new UsersManagementEntities())
            {
                context.User.Add(model);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public static string Encrypt(string textToBeEncrypted) 
        {
            if (!String.IsNullOrEmpty(textToBeEncrypted))
            {
                MD5 md5 = MD5.Create();
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(textToBeEncrypted);
                byte[] hash = md5.ComputeHash(bytes);

                StringBuilder sbPassword = new StringBuilder();
                for (int b = 0; b < hash.Length; b++)
                {
                    sbPassword.Append(hash[b].ToString("X2"));
                }

                return sbPassword.ToString();
            }
            else
            {
                return string.Empty;

            }
        }


    }
}