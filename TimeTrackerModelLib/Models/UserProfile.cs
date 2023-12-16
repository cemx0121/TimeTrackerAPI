﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeTrackerModelLib.Models
{
    [Table("UserProfile")]
    public partial class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        public int? UserId { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserProfiles")]
        public virtual User User { get; set; }
    }
}