﻿using System;
using System.ComponentModel.DataAnnotations;
using BusinessTrips.DAL.Attribute;
using BusinessTrips.DAL.Entity;
using BusinessTrips.DAL.Repository;
using BusinessTrips.DAL.Storage;

namespace BusinessTrips.DAL.Model.User
{
    public class UserRegistrationModel : PasswordRegistrationModel, IUserRegistrationModel
    {
        private const int MinimumNameLength = 3;
        private readonly IRandomStringGenerator randomStringGenerator;
        private readonly IUserRepository repository;

        [Required]
        [Display(Name = "Name")]
        [MinLength(MinimumNameLength, ErrorMessage = "Name length must be at least 3 characters long")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        [UniqueEmail(ErrorMessage = "This e-mail is already registered")]
        public string Email { get; set; }

        public UserRegistrationModel()
        {
            repository = new UserRepository();
            randomStringGenerator = new RandomStringGenerator();
        }

        public UserRegistrationModel(IRandomStringGenerator randomStringGenerator, IUserRepository userRepository)
        {
            repository = userRepository;
            this.randomStringGenerator = randomStringGenerator;
        }

        public void Save()
        {
            Id = Guid.NewGuid();

            UserEntity userEntity = ToUserEntity();
            userEntity.Roles.Add(new RoleRepository().GetRole(Roles.Regular.ToString()));
            userEntity.Salt = randomStringGenerator.GetSalt();
            userEntity.HashedPassword = PasswordHasher.GetHashed(Password + userEntity.Salt);

            repository.CreateByUserEntity(userEntity);
            repository.CommitChanges();
        }

        public UserEntity ToUserEntity()
        {
            return new UserEntity()
            {
                Name = Name,
                Email = Email,
                IsConfirmed = false,
                Id = Id
            };
        }
    }

    public interface IUserRegistrationModel
    {
    }
}