﻿using System;
using System.ComponentModel.DataAnnotations;
using BusinessTrips.DAL.Repository;

namespace BusinessTrips.DAL.Model.User
{
    public class ForgotPasswordModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        public void ToForgotPasswordModelByEmail(string email)
        {
           var userRepository=new UserRepository();
           var userEntity = userRepository.GetBy(email);
            Id = userEntity.Id;
        }

    }
}
