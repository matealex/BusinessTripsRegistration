﻿using System;
using System.ComponentModel.DataAnnotations;
using BusinessTrips.DAL.Entity;
using BusinessTrips.DAL.Repository;

namespace BusinessTrips.DAL.Model
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsConfirmed { get; set; }

        public bool Authenthicate()
        {
            var repository = new UserRepository();
            UserEntity userEntity = repository.GetByEmail(Email);

            if (userEntity == null)
            {
                return false;
            }

            Password = PasswordHasher.HashPassword(Password + userEntity.Salt);
            return repository.AreCredentialsValid(Email, Password);
        }

        public UserEntity ToEntity()
        {
            return new UserEntity()
            {
                Name = Name,
                Email = Email,
                IsConfirmed = IsConfirmed,
                Id = Id,
                HashedPassword = Password,
            };
        }
    }
}
