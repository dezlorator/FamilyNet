using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObjects
{
    public class ChildrenActivityDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<AwardDTO> Awards { get; set; }
        public int ChildID { get; set; }
    }
}
