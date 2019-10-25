using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;


namespace FamilyNetServer.Models
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

                #region Volunteer1

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

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer2

                volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Иван", Surname = "Новиков", Patronymic = "Русланович" };
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

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer3

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

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer4

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

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer5

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

                #endregion

                volunteers.Add(volunteer);

                _unitOfWorkAsync.Volunteers.AddRange(volunteers);
                _unitOfWorkAsync.SaveChangesAsync();
            }

            if (_unitOfWorkAsync.Orphanages.Get(v => v.ID == v.ID) != null)
            {
                List<Orphanage> orphanages = new List<Orphanage>();

                #region Orphanage1

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
                orphanage.Avatar = "3nf3pby0.jpg";

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

                #endregion

                orphanages.Add(orphanage);

                #region Orphanage2

                orphanage = new Orphanage();

                orphanage.Name = "Эльф";
                orphanage.Adress = new Address()
                {
                    Country = "Украина",
                    Region = "Сумская",
                    City = "Сумы",
                    Street = "Котляревського",
                    House = "89"
                };
                orphanage.Rating = 7;
                orphanage.Avatar = "jltx4rd2.jpg";

                representatives = new List<Representative>();
                representative = new Representative();

                representative.FullName = new FullName() { Name = "Марина", Surname = "Кричич", Patronymic = "Михайловна" };
                representative.Birthday = new DateTime(1954, 12, 30);
                representative.Rating = 5;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Анастасия", Surname = "Горб", Patronymic = "Андреевна" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);


                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Мария", Surname = "Павленко", Patronymic = "Александровна" };
                orphan.Birthday = new DateTime(2010, 8, 11);
                orphan.Rating = 7;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);
                orphanage.Orphans = orphans;

                #endregion

                orphanages.Add(orphanage);

                #region Orphanage3

                orphanage = new Orphanage();

                orphanage.Name = "Артек";
                orphanage.Adress = new Address()
                {
                    Country = "Украина",
                    Region = "Днепропетровская",
                    City = "Никополь",
                    Street = "Херсонская",
                    House = "177"
                };
                orphanage.Rating = 8;
                orphanage.Avatar = "proeundk.jpg";

                representative = new Representative();
                representatives = new List<Representative>();

                representative.FullName = new FullName() { Name = "Глеб", Surname = "Голосенин", Patronymic = "Иванович" };
                representative.Birthday = new DateTime(1988, 3, 4);
                representative.Rating = 5;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;


                orphans = new List<Orphan>();
                orphan = new Orphan();


                orphan.FullName = new FullName() { Name = "Арен", Surname = "Зурабян", Patronymic = "Кирилович" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Виталий", Surname = "Цаль", Patronymic = "Ильич" };
                orphan.Birthday = new DateTime(2010, 8, 11);
                orphan.Rating = 7;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphanage.Orphans = orphans;

                #endregion

                orphanages.Add(orphanage);

                #region Orphanage4

                orphanage = new Orphanage();

                orphanage.Name = "Нэвэрлэнд";
                orphanage.Adress = new Address()
                {
                    Country = "Украина",
                    Region = "Киевская",
                    City = "Белая Церковь",
                    Street = "Гаек",
                    House = "227"
                };
                orphanage.Rating = 3;
                orphanage.Avatar = "rwlmqppn.jpg";

                representative = new Representative();
                representatives = new List<Representative>();


                representative.FullName = new FullName() { Name = "Спартак", Surname = "Алексеев", Patronymic = "Андреевич" };
                representative.Birthday = new DateTime(1988, 3, 4);
                representative.Rating = 8;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Захар", Surname = "Мисра", Patronymic = "Владимирович" };
                orphan.Birthday = new DateTime(2011, 11, 14);
                orphan.Rating = 7;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphan = new Orphan();


                orphan.FullName = new FullName() { Name = "Анна", Surname = "Анисимова", Patronymic = "Сергеевна" };
                orphan.Birthday = new DateTime(2014, 7, 25);
                orphan.Rating = 9;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphanage.Orphans = orphans;

                #endregion

                orphanages.Add(orphanage);


                _unitOfWorkAsync.Orphanages.AddRange(orphanages);
                _unitOfWorkAsync.SaveChangesAsync();
            }

            if (_unitOfWorkAsync.CharityMakers.Get(v => v.ID == v.ID) != null)
            {

                List<CharityMaker> charityMakers = new List<CharityMaker>();

                #region CharityMaker1

                CharityMaker charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Bладимир", Surname = "Халин", Patronymic = "Тарасович" };
                charityMaker.Birthday = new DateTime(1991, 12, 3);
                charityMaker.Rating = 5;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Харьковская",
                    City = "Харьков",
                    Street = "Титова",
                    House = "22"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker2

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Сергей", Surname = "Калинич", Patronymic = "Евгеньевич" };
                charityMaker.Birthday = new DateTime(1972, 6, 12);
                charityMaker.Rating = 7;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Львовская",
                    City = "Львов",
                    Street = "Наукова",
                    House = "15"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker3

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Алена", Surname = "Чипко", Patronymic = "Максимовна" };
                charityMaker.Birthday = new DateTime(1993, 4, 28);
                charityMaker.Rating = 9;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Донецкая",
                    City = "Донецк",
                    Street = "Семашко",
                    House = "86"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker4

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Кристина", Surname = "Юдина", Patronymic = "Сергеевна" };
                charityMaker.Birthday = new DateTime(1984, 5, 18);
                charityMaker.Rating = 10;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Украина",
                    Region = "Одесская",
                    City = "Одесса",
                    Street = "Лютеранский Пер",
                    House = "52"
                };
                #endregion

                charityMakers.Add(charityMaker);

                _unitOfWorkAsync.CharityMakers.AddRange(charityMakers);
                _unitOfWorkAsync.SaveChangesAsync();
            }

            if (_unitOfWorkAsync.BaseItemTypes.Get(v => v.ID == v.ID) != null)
            {
                List<DonationItemType> baseItemTypes = new List<DonationItemType>();

                DonationItemType baseItemType = new DonationItemType();

                baseItemType.Name = "Обувь";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Куртки";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Штаны";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Футболки";
                baseItemTypes.Add(baseItemType);

                _unitOfWorkAsync.BaseItemTypes.AddRange(baseItemTypes);
                _unitOfWorkAsync.SaveChangesAsync();
            }

            AddOrphans();
        }

        public void AddOrphans()
        {
            if (_unitOfWorkAsync.Orphans.Get(o => o.ID == o.ID) != null)
            {
                #region orphan1

                var orphan1 = new Orphan();
                orphan1.FullName = new FullName() { Name = "Игорь", Surname = "Литвиненко", Patronymic = "Сергеевич" };
                orphan1.Birthday = new DateTime(2009, 5, 18);
                orphan1.Rating = 2.6F;
                orphan1.Avatar = "";
                orphan1.OrphanageID = 1;
                orphan1.Avatar = "Children/ИгорьЛитвиненкоСер637063178278846348.jpg";

                #endregion

                #region orphan2

                var orphan2 = new Orphan();
                orphan2.FullName = new FullName() { Name = "Виталий", Surname = "Титов", Patronymic = "Анатольевич" };
                orphan2.Birthday = new DateTime(2009, 3, 6);
                orphan2.Rating = 3.4F;
                orphan2.Avatar = "";
                orphan2.OrphanageID = 1;
                orphan2.Avatar = "Children/ВиталийТитовАнатольевич637063188603739335.jpg";

                #endregion

                #region orphan3

                var orphan3 = new Orphan();
                orphan3.FullName = new FullName() { Name = "Елена", Surname = "Титова", Patronymic = "Павловна" };
                orphan3.Birthday = new DateTime(2011, 9, 8);
                orphan3.Rating = 5.5F;
                orphan3.Avatar = "";
                orphan3.OrphanageID = 1;
                orphan3.Avatar = "Children/ЕленаТитоваПавловна637066549987669498.png";

                #endregion

                #region orphan4

                var orphan4 = new Orphan();
                orphan4.FullName = new FullName() { Name = "Светлана", Surname = "Петрова", Patronymic = "Викторовна" };
                orphan4.Birthday = new DateTime(2013, 12, 13);
                orphan4.Rating = 6.8F;
                orphan4.Avatar = "";
                orphan4.OrphanageID = 1;
                orphan4.Avatar = "Children/СветланаПетроваВикторовна637066556028402470.png";

                #endregion

                _unitOfWorkAsync.Orphans.AddRange(new List<Orphan>() { orphan1, orphan2, orphan3, orphan4 });
                _unitOfWorkAsync.SaveChangesAsync();
            }
        }
    }
}
