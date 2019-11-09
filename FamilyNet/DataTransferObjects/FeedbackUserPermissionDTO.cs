using DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    class FeedbackUserPermissionDTO
    {
        public UserRole UserRole { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
    }
}
