using leave_management.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace leave_management.Models
{
    public class LeaveRequestVM
    {
      
        public int ID { get; set; }
        public Employee RequestingEmployee { get; set; }
        [Display(Name = "Employee Name")]
        public string RequestingEmployeeId { get; set; }
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; }
        [Display(Name = "Date Actioned")]
        public DateTime DateActioned { get; set; }
        [Display(Name = "Approval Status")]
        public bool? Approved { get; set; }
        public Employee Approvedby { get; set; }

        [Display(Name = "Approver Name")]
        public string ApprovedById { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public bool? Cancelled { get; set; }
    }

    public class AdmistratorLeaveViewVM
    {
       [ Display(Name ="Total Number of Requests")]
        public int TotalRequests { get; set; }
        [Display(Name = "Approved Requests")]
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pending Requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }
        public List<LeaveRequestVM> leaverequests { get; set; }
    }

    public class CreateLeaveRequestVM
    {
        public int ID { get; set; }

        [Display(Name = "Start Date")]
        [Required]
       
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required]
        
        public string EndDate { get; set; }

        public IEnumerable<SelectListItem> LeaveTypes { get; set; }
        [Display(Name = "Leave Type")]

        public int LeaveTypeId { get; set; }
        [Display(Name = "Employee Comments")]
        [MaxLength(300)]
        public string Comment { get; set; }
    }

    public class EmployeeLeaveRequestView
    {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }

        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}
