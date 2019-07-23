﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Orphan : Person
    {
        public int? OrphanageID { get; set; }


        //[Required]
        public virtual Orphanage Orphanage { get; set; }

        public virtual ICollection<AuctionLot> AuctionLots { get; set; }


        public bool Confirmation { get; set; }
        public bool ChildInOrphanage { get; set; }

        public static void CopyState(Orphan receiver, Orphan sender)
        {
            receiver.FullName.Name = sender.FullName.Name;
            receiver.FullName.Surname = sender.FullName.Surname;
            receiver.FullName.Patronymic = sender.FullName.Patronymic;

            receiver.Rating = sender.Rating;
            receiver.Birthday = sender.Birthday;

            receiver.Orphanage = sender.Orphanage;

            receiver.Avatar = sender.Avatar;

        }

    }
}