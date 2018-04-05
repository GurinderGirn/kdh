﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace kdh.Models
{
    [Table("Job")]
    [MetadataType(typeof(buddy_Job))]
    public partial class Job
    {

    }
    public class buddy_Job
    {
        [Key]
        [Required(ErrorMessage = "Id field is required.")]
        [StringLength(50, ErrorMessage = "Input must be between 2 and 50 characters.", MinimumLength = 2)]
        [Display(Name = "Id")]
        public string JobId { get; set; }

        [Required(ErrorMessage = "Job title field is required.")]
        [StringLength(50, ErrorMessage = "Input must be between 5 and 50 characters.", MinimumLength = 5)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Job status field is required.")]
        [StringLength(50, ErrorMessage = "Input must be between 5 and 50 characters.", MinimumLength = 5)]
        [Display(Name = "Status")]
        public string JobStatus { get; set; }

        [Display(Name = "Description")]
        public string JobDescription { get; set; }

        [Required(ErrorMessage = "Please select a department.")]
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }

        [Required(ErrorMessage = "Date posted for job must be later than today.")]
        //[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date posted")]
        public Nullable<DateTime> DatePosted { get; set; }

        [Required(ErrorMessage = "Date closed for job must be later than today.")]
        //[DisplayFormat(DataFormatString = "{yyyy-mm-dd hh:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date closed")]
        public Nullable<DateTime> DateClosed { get; set; }

        [Display(Name = "Shift")]
        [StringLength(50, ErrorMessage = "Input must be between 5 and 50 characters.", MinimumLength = 5)]
        public string JobShift { get; set; }

        [Required(ErrorMessage = "Salary for job is required.")]
        [StringLength(50, ErrorMessage = "Input must be between 5 and 50 characters.", MinimumLength = 5)]
        [Display(Name = "Salary")]
        public string Salary { get; set; }

        [Required(ErrorMessage = "Job requirement field is required.")]
        [Display(Name = "Requirement")]
        public string Requirement { get; set; }

        [Required(ErrorMessage = "Job manager field is required.")]
        [Display(Name = "Manager's Email")]
        public int UserId { get; set; }
    }
}