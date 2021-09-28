using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeerepo;
        private readonly ILeaveRequestRepository _leaveRequestrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public HomeController
        (
            ILogger<HomeController> logger,
            IMapper mapper,
            UserManager<Employee> userManager,
            ILeaveTypeRepository leaveTyperepo,
            ILeaveAllocationRepository leaveAllocrepo,
            IEmployeeRepository employeerepo,
            ILeaveRequestRepository leaveRequestrepo
        )
        {
            _logger = logger;
            _employeerepo = employeerepo;
            _leaveRequestrepo = leaveRequestrepo;
            _mapper = mapper;
            userManager = _userManager;
        }

        public async Task<ActionResult> Index()
        {
            var leaverequests = await _leaveRequestrepo.FindAll();
            var leaverequestModel = _mapper.Map<List<LeaveRequestVM>>(leaverequests);
            var newuserreg = await _employeerepo.GetNewUserRegistration();
            var newuserregmodel = _mapper.Map<List<EmployeeVM>>(newuserreg);
            var model = new AdminDashBooardViewVM
            {
                PendingRequests = leaverequestModel.Count(q => q.Approved == null),
                NewUserRegistration = newuserregmodel.Count()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
