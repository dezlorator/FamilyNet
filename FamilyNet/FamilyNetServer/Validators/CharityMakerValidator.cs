﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;
using FamilyNetServer.Models.Interfaces;

namespace FamilyNetServer.Validators
{
    public class CharityMakerValidator : ICharityMakerValidator
    {
        public bool IsValid(CharityMakerDTO charityMakerDTO)
        {
            if (charityMakerDTO.Birthday == null ||
                String.IsNullOrEmpty(charityMakerDTO.Name) ||
                String.IsNullOrEmpty(charityMakerDTO.Surname) ||
                String.IsNullOrEmpty(charityMakerDTO.Patronymic) ||
                charityMakerDTO.CharityMakerID < 0)
            {
                return false;
            }

            return true;
        }
    }
}