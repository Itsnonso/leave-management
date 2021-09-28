using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestrepo;
        private readonly ILeaveTypeRepository _leaveTyperepo;
        private readonly ILeaveAllocationRepository _leaveAllocrepo;
        private readonly IEmployeeRepository _employeerepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        public EmployeeController
        (
            ILeaveRequestRepository leaveaRequestrepo,
            IMapper mapper,
            UserManager<Employee> userManager,
           ILeaveTypeRepository leaveTyperepo,
           ILeaveAllocationRepository leaveAllocrepo,
           IEmployeeRepository employeerepo
        )
        {
            _leaveRequestrepo = leaveaRequestrepo;
            _mapper = mapper;
            _userManager = userManager;
            _leaveTyperepo = leaveTyperepo;
            _leaveAllocrepo = leaveAllocrepo;
            _employeerepo = employeerepo;
        }
        [Authorize(Roles = "Administrator")]
        // GET: EmployeeController
        public async Task< ActionResult> Index()
        {
            var leaverequests = await _leaveRequestrepo.FindAll();
            var leaverequestModel = _mapper.Map<List<LeaveRequestVM>>(leaverequests);
            var newuserreg = await _employeerepo.GetNewUserRegistration();
            var newuserregmodel = _mapper.Map<List<EmployeeVM>>(newuserreg);
            var model = new AdminDashBooardViewVM
            {
                PendingRequests =  leaverequestModel.Count(q => q.Approved == null),
                NewUserRegistration = newuserregmodel.Count()
            };
            return View(model);
        }

        // GET: EmployeeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
