//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kdh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Doctor
    {
        public int Doctorid { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string Address_line1 { get; set; }
        public string Address_line2 { get; set; }
        public string Postal_code { get; set; }
        public string Mobile_no { get; set; }
        public Nullable<System.DateTime> Date_of_join { get; set; }
        public int Departmentid { get; set; }
        public string Speciality { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }
}
