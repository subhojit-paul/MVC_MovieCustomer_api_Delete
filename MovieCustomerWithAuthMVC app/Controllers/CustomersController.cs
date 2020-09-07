using MovieCustomerWithAuthMVC_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieCustomerWithAuthMVC_app.Models.ViewModel;
using System.Data.Entity;
using System.Net;
using System.IO;

namespace MovieCustomerWithAuthMVC_app.Controllers
{
    public class CustomersController : Controller
    {
       
       // GET: Customers
        private ApplicationDbContext _context;
        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Customers
        public ActionResult Index()
        {
            var customers = _context.Customers.Include(c => c.MembershipType).ToList();
            return View(customers);
        }
        public ActionResult Details(int id)
        {
            var singleCustomer = _context.Customers.Include(c => c.MembershipType).SingleOrDefault(c => c.Id == id);
            if (singleCustomer == null)
                return HttpNotFound();
            return View(singleCustomer);
        }
        [HttpGet]
        public ActionResult New()  //display the form
        {
            var membershipTypes = _context.MembershipTypes.ToList();
            var viewModel = new NewCustomerViewModel
            {
                MembershipTypes = membershipTypes
            };
            return View(viewModel);
        }
        //[HttpPost]
        //public ActionResult Create(Customer customer)  //submit the form,parameter is of model
        //                                               //must have parameter in Post method      //Its called Model Binding
        //{
        //    _context.Customers.Add(customer);
        //    _context.SaveChanges();
        //    return RedirectToAction("Index", "Customers");//Back to customer controller page
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new NewCustomerViewModel
                {
                    Customer = customer,
                    MembershipTypes = _context.MembershipTypes.ToList()
                };
                return View("New", viewModel);
            }


            if (customer.Id == 0)
                _context.Customers.Add(customer);
            else
            {
                var customerInDb = _context.Customers.Single(c => c.Id == customer.Id);
                customerInDb.Name = customer.Name;
                customerInDb.DOB = customer.DOB;
                customerInDb.MembershipTypeId = customer.MembershipTypeId;
                customerInDb.ISSubscribedToNewsLetter = customer.ISSubscribedToNewsLetter;



            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Customers");
        }



        public ActionResult Edit(int id)
        {
            var updateCast = _context.Customers.SingleOrDefault(c => c.Id == id);
            if (updateCast == null)
            {
                return HttpNotFound();
            }
            var vm = new NewCustomerViewModel
            {
                Customer = updateCast,
                MembershipTypes = _context.MembershipTypes.ToList()
            };
            return View("New",vm);
        }




        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customerTbl = _context.Customers.Find(id);
            _context.Customers.Remove(customerTbl);
            _context.SaveChanges();
            return RedirectToAction("Index", "Customers");
        }





        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}