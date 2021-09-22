using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repo, IMapper mapper, IUnitofWork unitofwork)
        {
            _repo = repo;
            _unitofwork = unitofwork;
            _mapper = mapper;
        }
        // GET: LeaveTypeController
        public async Task<ActionResult> Index()
        {
            //var leavetypes = await _repo.FindAll();
            var leavetypes = await _unitofwork.LeaveTypes.FindAll();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList());

            return View(model);
        }

        // GET: LeaveTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            // var isExists = await _repo.isExists(id);
            var isExists = await _unitofwork.LeaveTypes.isExists(q => q.ID == id);
            if (!isExists)
            {
                return NotFound();
            }
            //var leavetype = await _repo.FindById(id);
            var leavetype = await _unitofwork.LeaveTypes.Find(q => q.ID == id);
            var model = _mapper.Map<LeaveTypeVM>(leavetype);
            return View(model);
        }

        // GET: LeaveTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leavetype = _mapper.Map<LeaveType>(model);
                leavetype.DateCreated = DateTime.Now;

                // var isSuccess = await _repo.Create(leavetype);
                await _unitofwork.LeaveTypes.Create(leavetype);

                await _unitofwork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypeController/Edit/5
        public async Task< ActionResult> Edit(int id)
        {
           // var isExists = await _repo.isExists(id);
            var isExists = await _unitofwork.LeaveTypes.isExists(q => q.ID == id);
            if (!isExists)
            {
                return NotFound();
            }
            var leavetype = await _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leavetype);
            return View(model);
        }

        // POST: LeaveTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leavetype = _mapper.Map<LeaveType>(model);
                //var isSuccess = await _repo.Update(leavetype);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went wrong...");
                //    return View(model);
                //}
                _unitofwork.LeaveTypes.Update(leavetype);
                await _unitofwork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypeController/Delete/5
        public async Task < ActionResult> Delete(int id)
        {
            var leavetype = await _repo.FindById(id);
            if (leavetype == null)
            {
                return NotFound();
            }
            var isSuccess = await _repo.Delete(leavetype);
            if (!isSuccess)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index)); 
        }

        // POST: LeaveTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Delete(int id, LeaveTypeVM model)
        {
            try
            {
                // var leavetype = await _repo.FindById(id);
                var leavetype = await _unitofwork.LeaveTypes.Find(expression: q => q.ID == id);
                if (leavetype == null)
                {
                    return NotFound();
                }
                 _unitofwork.LeaveTypes.Delete(leavetype);
    await _unitofwork.Save();
              //  var isSuccess = await _repo.Delete(leavetype);
                //if (!isSuccess)
                //{
                //    ModelState.AddModelError("", "Something went ..");
                //    return View(model);
                //}
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
