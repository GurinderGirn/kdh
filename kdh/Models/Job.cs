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
    
    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Job()
        {
            this.Applicants = new HashSet<Applicant>();
        }
    
        public string JobId { get; set; }
        public string JobTitle { get; set; }
        public string JobStatus { get; set; }
        public string JobDescription { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<System.DateTime> DatePosted { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public string JobShift { get; set; }
        public string Salary { get; set; }
        public string Requirement { get; set; }
        public System.Guid UserId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual department department { get; set; }
        public virtual User User { get; set; }
    }
}
