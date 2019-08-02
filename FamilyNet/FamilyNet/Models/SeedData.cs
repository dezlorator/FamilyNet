using FamilyNet.Models.EntityFramework;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class SeedData
    {
        protected IUnitOfWorkAsync _unitOfWorkAsync;

        public SeedData(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWorkAsync = unitOfWork;
        }
        public void EnsurePopulated()
        {

            if (_unitOfWorkAsync.Volunteers.Get(v => v.ID == v.ID) != null)
            {
            List<Volunteer> volunteers = new List<Volunteer>();

                Volunteer volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Anna", Surname = "Бутенко", Patronymic = "Андреевна" };
                volunteer.Birthday = new DateTime(1998, 11, 30);
                volunteer.Rating = 8;
                volunteer.Avatar = "ewwwee";
                volunteer.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Днепропетровская",
                    City = "Новомосковск",
                    Street = "Школьная",
                    House = "4"
                };

                volunteers.Add(volunteer);

                volunteer = new Volunteer();
                volunteer.FullName = new FullName(){ Name = "Иван", Surname = "Новиков", Patronymic = "Русланович" };
                volunteer.Birthday = new DateTime(1991, 4, 10);
                volunteer.Rating = 3;
                volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Днепропетровская",
                    City = "Днепр",
                    Street = "Козацкая",
                    House = "14"
                };

                volunteers.Add(volunteer);

                volunteer = new Volunteer();
                
                   volunteer.FullName = new FullName() { Name = "Михаил", Surname = "Иванов", Patronymic = "Сергеевич" };
                   volunteer.Birthday = new DateTime(1993, 12, 25);
                   volunteer.Rating = 4;
                   volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Киевская",
                    City = "Киев",
                    Street = "Героев",
                    House = "111"
                };

                volunteers.Add(volunteer);

                volunteer = new Volunteer();
                    volunteer.FullName = new FullName() { Name = "Марина", Surname = "Романчук", Patronymic = "Сергеевна" };
                    volunteer.Birthday = new DateTime(1999, 1, 20);
                    volunteer.Rating = 7;
                    volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Днепропетровская",
                    City = "Днепр",
                    Street = "Гагарина",
                    House = "63"
                };

                volunteers.Add(volunteer);

                volunteer = new Volunteer();
                    volunteer.FullName = new FullName() { Name = "Алина", Surname = "Лейко", Patronymic = "Олеговна" };
                    volunteer.Birthday = new DateTime(1985, 4, 1);
                    volunteer.Rating = 10;
                    volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Киевская",
                    City = "Киев",
                    Street = "Восьмого Марта",
                    House = "17"
                };

                volunteers.Add(volunteer);

                _unitOfWorkAsync.Volunteers.AddRange(volunteers);
                _unitOfWorkAsync.SaveChangesAsync();
            }

            if (_unitOfWorkAsync.Orphanages.Get(v => v.ID == v.ID) != null)
            {
                List<Orphanage> orphanages = new List<Orphanage>();

                Orphanage orphanage = new Orphanage();

                orphanage.Name = "Ромашка";
                orphanage.Adress = new Address()
                {
                    Country = "Украина",
                    Region = "Херсонская",
                    City = "Херсон",
                    Street = "Дровника",
                    House = "25"
                };
                orphanage.Rating = 11;
                orphanage.Avatar = "";

                List<Representative> representatives = new List<Representative>();

                Representative representative = new Representative();
                
                representative.FullName = new FullName() { Name = "Олег", Surname = "Петренко", Patronymic = "Дмитреевич" };
                representative.Birthday = new DateTime(1954, 12, 30);
                representative.Rating = 5;
                representative.Avatar = "";
                
                representatives.Add(representative);
                orphanage.Representatives = representatives;

                List<Orphan> orphans = new List<Orphan>();

                Orphan orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Глеб", Surname = "Левада", Patronymic = "Русланович" };
                orphan.Birthday = new DateTime(2002, 2, 4);
                orphan.Rating = 5;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;
            
                orphans.Add(orphan);

                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Олег", Surname = "Курасов", Patronymic = "Михайлович" };
                orphan.Birthday = new DateTime(2001, 4, 12);
                orphan.Rating = 6;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);
                orphanage.Orphans = orphans;

                orphanages.Add(orphanage);

                _unitOfWorkAsync.Orphanages.AddRange(orphanages);
                _unitOfWorkAsync.SaveChangesAsync();


                //    new Orphanage
                //    {
                //        Name = "Эльф",
                //        Adress =
                //        {
                //           Country = "Украина",
                //           Region = "Сумская",
                //           City = "Сумы",
                //           Street = "Котляревського",
                //           House = "89"
                //        },
                //        Rating = 7,
                //        Avatar = "",
                //        Representatives = {
                //            new Representative{
                //                FullName = { Name = "Марина", Surname = "Кричич", Patronymic = "Михайловна" },
                //                Birthday = new DateTime(1954, 12, 30),
                //                Rating = 5,
                //                Avatar = ""
                //            }},
                //        Orphans = {
                //            new Orphan
                //            {
                //                FullName={Name = "Анастасия", Surname = "Горб", Patronymic = "Андреевна" },
                //                Birthday = new DateTime(2004, 5, 14),
                //                Rating = 8,
                //                Avatar = "",
                //                ChildInOrphanage=true,
                //                Confirmation=true
                //            },
                //            new Orphan
                //            {
                //                FullName={Name = "Мария", Surname = "Павленко", Patronymic = "Александровна" },
                //                Birthday = new DateTime(2010, 8, 11),
                //                Rating = 7,
                //                Avatar = "",
                //                ChildInOrphanage=true,
                //                Confirmation=true
                //            }
                //        },
                //        MapCoordX = 134,
                //        MapCoordY = 333
                //    },

                //    new Orphanage
                //    {
                //        Name = "Артек",
                //        Adress =
                //      {
                //        Country = "Украина",
                //        Region = "Днепропетровская",
                //        City = "Никополь",
                //        Street = "Херсонская",
                //        House = "177"
                //      },
                //        Rating = 8,
                //        Avatar = "",
                //        Representatives = {
                //          new Representative{
                //              FullName = { Name = "Глеб", Surname = "Голосенин", Patronymic = "Иванович" },
                //              Birthday = new DateTime(1988, 3, 4),
                //              Rating = 5,
                //              Avatar = ""
                //          }},
                //        Orphans = {
                //          new Orphan
                //          {
                //              FullName={Name = "Арен", Surname = "Зурабян", Patronymic = "Кирилович" },
                //              Birthday = new DateTime(2004, 5, 14),
                //              Rating = 8,
                //              Avatar = "",
                //              ChildInOrphanage=true,
                //              Confirmation=true
                //          },
                //          new Orphan
                //          {
                //              FullName={Name = "Виталий", Surname = "Цаль", Patronymic = "Ильич" },
                //              Birthday = new DateTime(2010, 8, 11),
                //              Rating = 7,
                //              Avatar = "",
                //              ChildInOrphanage=true,
                //              Confirmation=true
                //          }
                //      },
                //        MapCoordX = 53654,
                //        MapCoordY = 32342
                //    },

                //    new Orphanage
                //    {
                //        Name = "Нэвэрлэнд",
                //        Adress =
                //      {
                //        Country = "Украина",
                //        Region = "Киевская",
                //        City = "Белая Церковь",
                //        Street = "Гаек",
                //        House = "227"
                //      },
                //        Rating = 3,
                //        Avatar = "",
                //        Representatives = {
                //          new Representative{
                //              FullName = { Name = "Спартак", Surname = "Алексеев", Patronymic = "Андреевич" },
                //              Birthday = new DateTime(1988, 3, 4),
                //              Rating = 8,
                //              Avatar = ""
                //          }},
                //        Orphans = {
                //          new Orphan
                //          {
                //              FullName={Name = "Захар", Surname = "Мисра", Patronymic = "Владимирович" },
                //              Birthday = new DateTime(2011, 11, 14),
                //              Rating = 7,
                //              Avatar = "",
                //              ChildInOrphanage=true,
                //              Confirmation=true
                //          },
                //          new Orphan
                //          {
                //              FullName={Name = "Анна", Surname = "Анисимова", Patronymic = "Сергеевна" },
                //              Birthday = new DateTime(2014, 7, 25),
                //              Rating = 9,
                //              Avatar = "",
                //              ChildInOrphanage=true,
                //              Confirmation=true
                //          }
                //      },
                //        MapCoordX = 32442,
                //        MapCoordY = 12352
                //    }
                //    );
                //context.SaveChanges();
            }

            //if (!context.CharityMakers.Any())
            //{
            //    context.AddRange(

            //        new CharityMaker
            //        {
            //            FullName = { Name = "Bладимир", Surname = "Халин", Patronymic = "Тарасович" },
            //            Birthday = new DateTime(1991, 12, 3),
            //            Rating = 5,
            //            Avatar = "",
            //            Address = {
            //                 Country = "Украина",
            //                 Region = "Харьковская",
            //                 City = "Харьков",
            //                 Street = "Титова",
            //                 House = "22"
            //            }
            //        },

            //        new CharityMaker
            //        {
            //            FullName = { Name = "Сергей", Surname = "Калинич", Patronymic = "Евгеньевич" },
            //            Birthday = new DateTime(1972, 6, 12),
            //            Rating = 7,
            //            Avatar = "",
            //            Address = {
            //                 Country = "Украина",
            //                 Region = "Львовская",
            //                 City = "Львов",
            //                 Street = "Наукова",
            //                 House = "15"
            //           }
            //        },

            //        new CharityMaker
            //        {
            //            FullName = { Name = "Алена", Surname = "Чипко", Patronymic = "Максимовна" },
            //            Birthday = new DateTime(1993, 4, 28),
            //            Rating = 9,
            //            Avatar = "",
            //            Address = {
            //                 Country = "Украина",
            //                 Region = "Донецкая",
            //                 City = "Донецк",
            //                 Street = "Семашко",
            //                 House = "86"
            //           }
            //        },

            //        new CharityMaker
            //        {
            //            FullName = { Name = "Кристина", Surname = "Юдина", Patronymic = "Сергеевна" },
            //            Birthday = new DateTime(1984, 5, 18),
            //            Rating = 10,
            //            Avatar = "",
            //            Address = {
            //                 Country = "Украина",
            //                 Region = "Одесская",
            //                 City = "Одесса",
            //                 Street = "Лютеранский Пер",
            //                 House = "52"
            //           }
            //        }
            //        );
            //    context.SaveChanges();
            //}
        }
    }
}
