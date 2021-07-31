using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestrepo;
        private readonly ILeaveTypeRepository _leaveTyperepo;
        private readonly ILeaveAllocationRepository _leaveAllocrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController
            (
            ILeaveRequestRepository leaveaRequestrepo, 
            IMapper mapper, 
            UserManager<Employee> userManager,
           ILeaveTypeRepository leaveTyperepo, 
           ILeaveAllocationRepository leaveAllocrepo
            )
        {
            _leaveRequestrepo = leaveaRequestrepo;
            _mapper = mapper;
            _userManager = userManager;
            _leaveTyperepo = leaveTyperepo;
            _leaveAllocrepo = leaveAllocrepo;
        }
        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public ActionResult Index()
        {
            var leaverequests = _leaveRequestrepo.FindAll();
            var leaverequestModel = _mapper.Map<List<LeaveRequestVM>>(leaverequests);
            var model = new AdmistratorLeaveViewVM
            {
                TotalRequests = leaverequestModel.Count,
                PendingRequests = leaverequestModel.Count(q => q.Approved == null),
                RejectedRequests = leaverequestModel.Count(q => q.Approved == false),
                ApprovedRequests = leaverequestModel.Count(q => q.Approved == true),
                leaverequests = leaverequestModel
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequestrepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);

            return View(model);
            
        }

        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeid = employee.Id;
            var employeeallocations = _leaveAllocrepo.GetLeaveAllocationbyEmployee(employeeid);
            var employeerequests = _leaveRequestrepo.GetLeaveRequestbyEmployee(employeeid);

            var employeeallocationModel = _mapper.Map<List<LeaveAllocationVM>>(employeeallocations);
            var employeerequestmodel = _mapper.Map<List<LeaveRequestVM>>(employeerequests);

            var model = new EmployeeLeaveRequestView
            {
                LeaveAllocations = employeeallocationModel,

                LeaveRequests = employeerequestmodel
            };

            return View(model);
        }
        public ActionResult ApproveRequest(int id) 
        {
            try
            {
                var employee = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestrepo.FindById(id);
                leaveRequest.Approved = true;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = employee.Id;
                var employeeid = leaveRequest.RequestingEmployeeId;
                var leavetypeid = leaveRequest.LeaveTypeId;
                var allocation = _leaveAllocrepo.GetLeaveAllocationbyEmployeeandType(employeeid, leavetypeid);

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                allocation.NumberofDays -= daysRequested;

                 _leaveRequestrepo.Update(leaveRequest);
                _leaveAllocrepo.Update(allocation);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
           
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                var employee = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestrepo.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.DateActioned = DateTime.Now;
                leaveRequest.ApprovedById = employee.Id;

                var Issuccesful = _leaveRequestrepo.Update(leaveRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult CancelRequest(int id)
        {
            try
            {
                var employee = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestrepo.FindById(id);
                leaveRequest.Cancelled = true;

                 _leaveRequestrepo.Update(leaveRequest);
                return RedirectToAction("MyLeave"); 
            }
            catch (Exception)
            {
                return RedirectToAction("MyLeave");
            }
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leavetypes = _leaveTyperepo.FindAll();
            var leavetypeItems = leavetypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.ID.ToString()
            });
            var model = new CreateLeaveRequestVM
            { 
                LeaveTypes = leavetypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate, new CultureInfo("en-UK"));
                var endDate = Convert.ToDateTime(model.EndDate, new CultureInfo("en-UK"));
                var todaydate = Convert.ToDateTime(DateTime.Now, new CultureInfo("en-UK"));
                var leavetypes = _leaveTyperepo.FindAll();
                var leavetypeItems = leavetypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.ID.ToString()
                });

                model.LeaveTypes = leavetypeItems;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (DateTime.Compare(startDate,todaydate) <= 0 )
                {
                    ModelState.AddModelError("", "Start Date must be ahead of Today's date...");
                    return View(model);
                }
                if (DateTime.Compare(endDate, todaydate) < 0)
                {
                    ModelState.AddModelError("", "End Date cannot be earlier than today's date...");
                    return View(model);
                }
                if (DateTime.Compare(startDate,endDate) > 0)
                {
                    ModelState.AddModelError("", "Start Date cannot be further in the future than the End Date...");
                    return View(model);
                }
               
                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveAllocrepo.GetLeaveAllocationbyEmployeeandType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(endDate - startDate).TotalDays;

                if (daysRequested > allocation.NumberofDays)
                {
                    ModelState.AddModelError("", "Days requested exceed the number of days available to you");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    DateRequested = DateTime.Now,
                    StartDate = startDate,
                    EndDate = endDate,
                    DateActioned = DateTime.Now,
                    Approved = null,
                    LeaveTypeId = model.LeaveTypeId,
                    Comment = model.Comment,
                    Cancelled = false
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccessful = _leaveRequestrepo.Create(leaveRequest);

                if (!isSuccessful)
                {
                    ModelState.AddModelError("", "Something went wrong with submitting the record...");
                    return View(model);
                }

                return RedirectToAction("MyLeave");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(" ", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
