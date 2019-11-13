using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace FamilyNetServer.Models
{
    public class Representative : Person
    {
        public int OrphanageID { get; set; }

        [Display(Name="Детдом")]
        public virtual Orphanage Orphanage { get; set; }

        public override void CopyState(Person sender)
        {
            base.CopyState(sender);
            Representative representativeSent = sender as Representative;
            Orphanage = representativeSent.Orphanage; // TODO : Test - do is work right?
        }
    }
}