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
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        // GET: LeaveAllocationController
        private readonly ILeaveTypeRepository _leavetyperepo;
        private readonly ILeaveAllocationRepository _leaveallocationrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        public LeaveAllocationController(ILeaveTypeRepository leavetyperepo, ILeaveAllocationRepository leaveallocationrepo, IMapper mapper,UserManager<Employee> userMmanager)
        {
            _leaveallocationrepo = leaveallocationrepo;
            _leavetyperepo = leavetyperepo;
            _mapper = mapper;
            _userManager = userMmanager;
        }
        public async Task< ActionResult> Index()
        {
            var check = await _leavetyperepo.FindAll();
            var leavetypes = check.ToList();

            var mappedleavetypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes);
            var model = new CreateLeaveAllocationVM 
            { 
                LeaveTypes = mappedleavetypes,
                NumberUpdated = 0
            };
            return View(model);
        }
        public async Task<ActionResult> SetLeave(int id)
        {
            var leavetype = await _leavetyperepo.FindById(id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                if ( await _leaveallocationrepo.CheckAllocation(id, emp.Id)) 
                continue;
                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    NumberofDays = leavetype.DefaultDays,
                    EmployeeId = emp.Id,
                    Period = DateTime.Now.Year,
                    LeaveTypeId = leavetype.ID
                };
                var leaveallocation = _mapper.Map<LeaveAllocation>(allocation);
                await _leaveallocationrepo.Create(leaveallocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }
        // GET: LeaveAllocationController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>( await _userManager.FindByIdAsync(id));
            var allocations = _mapper.Map<List<LeaveAllocationVM>>(await _leaveallocationrepo.GetLeaveAllocationbyEmployee(id));
            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocations
            };
            return View(model);
        }

        // GET: LeaveAllocationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocationController/Create
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

        // GET: LeaveAllocationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var leaveallocation = await _leaveallocationrepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveallocation);
            return View(model);
        }

        // POST: LeaveAllocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var record = await _leaveallocationrepo.FindById(model.ID);
                record.NumberofDays = model.NumberofDays;
                var isSuccess = await _leaveallocationrepo.Update(record);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong...");
                    return View(model);
                }
                return RedirectToAction(nameof(Details), new { id = model.EmployeeId});
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveAllocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocationController/Delete/5
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
