using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;


namespace FamilyNetServer.Models
{
    public class SeedData
    {
        protected IUnitOfWork _unitOfWork;

        public SeedData(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void EnsurePopulated()
        {
            if (_unitOfWork.Orphanages.Get(v => v.ID == v.ID) != null)
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
                representative.Avatar = "avatars/seddata_representatives2.jpg";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                List<Orphan> orphans = new List<Orphan>();

                Orphan orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Глеб", Surname = "Левада", Patronymic = "Русланович" };
                orphan.Birthday = new DateTime(2002, 2, 4);
                orphan.Rating = 5;
                orphan.Avatar = "/avatars/seeddata_child1.jpg";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Олег", Surname = "Курасов", Patronymic = "Михайлович" };
                orphan.Birthday = new DateTime(2001, 4, 12);
                orphan.Rating = 6;
                orphan.Avatar = "/avatars/seeddata_child1.jpg";
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
                representative.Avatar = "avatars/seddata_representatives1.jpg";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Анастасия", Surname = "Горб", Patronymic = "Андреевна" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "/avatars/seeddata_child5.jpg";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);


                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Мария", Surname = "Павленко", Patronymic = "Александровна" };
                orphan.Birthday = new DateTime(2010, 8, 11);
                orphan.Rating = 7;
                orphan.Avatar = "/avatars/seeddata_child6.jpg";
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
                representative.Avatar = "avatars/seddata_representatives3.jpg";

                representatives.Add(representative);
                orphanage.Representatives = representatives;


                orphans = new List<Orphan>();
                orphan = new Orphan();


                orphan.FullName = new FullName() { Name = "Арен", Surname = "Зурабян", Patronymic = "Кирилович" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "/avatars/seeddata_child4.jpg";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Виталий", Surname = "Цаль", Patronymic = "Ильич" };
                orphan.Birthday = new DateTime(2010, 8, 11);
                orphan.Rating = 7;
                orphan.Avatar = "/avatars/seeddata_child4.jpg";
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
                representative.Avatar = "avatars/seddata_representatives4.jpg";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Захар", Surname = "Мисра", Patronymic = "Владимирович" };
                orphan.Birthday = new DateTime(2011, 11, 14);
                orphan.Rating = 7;
                orphan.Avatar = "/avatars/seeddata_child4.jpg";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphan = new Orphan();


                orphan.FullName = new FullName() { Name = "Анна", Surname = "Анисимова", Patronymic = "Сергеевна" };
                orphan.Birthday = new DateTime(2014, 7, 25);
                orphan.Rating = 9;
                orphan.Avatar = "/avatars/seeddata_child5.jpg";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphanage.Orphans = orphans;

                #endregion

                orphanages.Add(orphanage);


                _unitOfWork.Orphanages.AddRange(orphanages);
                _unitOfWork.SaveChangesAsync();
            }

            if (_unitOfWork.Volunteers.Get(v => v.ID == v.ID) != null)
            {
                List<Volunteer> volunteers = new List<Volunteer>();

                #region Volunteer1

                Volunteer volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Анна", Surname = "Бутенко", Patronymic = "Андреевна" };
                volunteer.Birthday = new DateTime(1998, 11, 30);
                volunteer.Rating = 8;
                volunteer.Avatar = "avatars/seeddata_0.jpg";
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
                volunteer.FullName = new FullName() { Name = "Иван", Surname = "Кенобов", Patronymic = "Русланович" };
                volunteer.Birthday = new DateTime(1991, 4, 10);
                volunteer.Rating = 3;
                volunteer.Avatar = "avatars/seeddata_1.jpg";
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

                volunteer.FullName = new FullName() { Name = "Николай", Surname = "Ривский", Patronymic = "Сергеевич" };
                volunteer.Birthday = new DateTime(1993, 12, 25);
                volunteer.Rating = 4;
                volunteer.Avatar = "avatars/seeddata_2.jpg";
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
                volunteer.FullName = new FullName() { Name = "Евгения", Surname = "Романчук", Patronymic = "Сергеевна" };
                volunteer.Birthday = new DateTime(1999, 1, 20);
                volunteer.Rating = 7;
                volunteer.Avatar = "avatars/seeddata_3.jpg";
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
                volunteer.FullName = new FullName() { Name = "Александр", Surname = "Бинов", Patronymic = "Олегович" };
                volunteer.Birthday = new DateTime(1985, 4, 1);
                volunteer.Rating = 10;
                volunteer.Avatar = "avatars/seeddata_4.jpg";
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

                #region Volunteer6

                volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Александр", Surname = "Джеков", Patronymic = "Петрович" };
                volunteer.Birthday = new DateTime(1987, 3, 10);
                volunteer.Rating = 10;
                volunteer.Avatar = "avatars/seeddata_5.jpg";
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

                _unitOfWork.Volunteers.AddRange(volunteers);
                _unitOfWork.SaveChangesAsync();
            }

            if (_unitOfWork.Orphans.Get(v => v.ID == v.ID) != null)
            {
                List<Orphan> orphans = new List<Orphan>();

                #region child1
                Orphan orphan1 = new Orphan();
                orphan1.FullName = new FullName() { Name = "Иван", Surname = "Бутенко", Patronymic = "Андреевич" };
                orphan1.Birthday = new DateTime(2010, 11, 30);
                orphan1.Rating = 3;
                orphan1.Avatar = "avatars/seeddata_child1.jpg";
                orphan1.OrphanageID = 1;

                #endregion

                orphans.Add(orphan1);

                #region child2
                Orphan orphan2 = new Orphan();
                orphan2.FullName = new FullName() { Name = "Виталий", Surname = "Гнатенко", Patronymic = "Богданович" };
                orphan2.Birthday = new DateTime(2011, 7, 12);
                orphan2.Rating = 5;
                orphan2.Avatar = "avatars/seeddata_child2.jpg";
                orphan2.OrphanageID = 3;
                #endregion

                orphans.Add(orphan2);

                #region child3
                Orphan orphan3 = new Orphan();
                orphan3.FullName = new FullName() { Name = "Павел", Surname = "Логвин", Patronymic = "Данилович" };
                orphan3.Birthday = new DateTime(2009, 3, 3);
                orphan3.Rating = 1;
                orphan3.Avatar = "avatars/seeddata_child3.jpg";
                orphan3.OrphanageID = 3;
                #endregion

                orphans.Add(orphan3);

                #region child4
                Orphan orphan4 = new Orphan();
                orphan4.FullName = new FullName() { Name = "Влад", Surname = "Петренко", Patronymic = "Гаврилович" };
                orphan4.Birthday = new DateTime(2012, 7, 1);
                orphan4.Rating = 6;
                orphan4.Avatar = "avatars/seeddata_child4.jpg";
                orphan4.OrphanageID = 2;
                #endregion

                orphans.Add(orphan4);

                #region child5
                Orphan orphan5 = new Orphan();
                orphan5.FullName = new FullName() { Name = "Инна", Surname = "Овеченко", Patronymic = "Ивановна" };
                orphan5.Birthday = new DateTime(2008, 8, 19);
                orphan5.Rating = 8;
                orphan5.Avatar = "avatars/seeddata_child5.jpg";
                orphan5.OrphanageID = 3;
                #endregion

                orphans.Add(orphan5);

                #region child6
                Orphan orphan6 = new Orphan();
                orphan6.FullName = new FullName() { Name = "Дарина", Surname = "Пушненко", Patronymic = "Константиновна" };
                orphan6.Birthday = new DateTime(2009, 2, 27);
                orphan6.Rating = 4;
                orphan6.Avatar = "avatars/seeddata_child6.jpg";
                orphan6.OrphanageID = 4;
                #endregion

                orphans.Add(orphan6);

                _unitOfWork.Orphans.AddRange(orphans);
                _unitOfWork.SaveChangesAsync();
            }

            if (_unitOfWork.CharityMakers.Get(v => v.ID == v.ID) != null)
            {

                List<CharityMaker> charityMakers = new List<CharityMaker>();

                #region CharityMaker1

                CharityMaker charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Константин", Surname = "Истенко", Patronymic = "Тарасович" };
                charityMaker.Birthday = new DateTime(1930, 12, 3);
                charityMaker.Rating = 6;
                charityMaker.Avatar = "avatars/seeddata_KleenEastwood.jpg";
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

                charityMaker.FullName = new FullName() { Name = "Хидео", Surname = "Кодзима", Patronymic = "Петрович" };
                charityMaker.Birthday = new DateTime(1972, 6, 12);
                charityMaker.Rating = 7;
                charityMaker.Avatar = "avatars/seeddata_KodzimaGeniy.png";
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

                charityMaker.FullName = new FullName() { Name = "Анатолий", Surname = "Ульфрик", Patronymic = "Скайримович" };
                charityMaker.Birthday = new DateTime(1983, 4, 28);
                charityMaker.Rating = 9;
                charityMaker.Avatar = "avatars/seeddata_KypiSkairim.png";
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

                charityMaker.FullName = new FullName() { Name = "Тор", Surname = "Асгардов", Patronymic = "Одинович" };
                charityMaker.Birthday = new DateTime(1, 1, 1);
                charityMaker.Rating = 10;
                charityMaker.Avatar = "avatars/seeddata_maxresdefault.jpg";
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

                #region CharityMaker5

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Иван", Surname = "Морфеус", Patronymic = "Иванов" };
                charityMaker.Birthday = new DateTime(1980, 8, 12);
                charityMaker.Rating = 10;
                charityMaker.Avatar = "avatars/seeddata_morfeys.jpg";
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

                #region CharityMaker6

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Валерий", Surname = "Кузнецов", Patronymic = "Иванов" };
                charityMaker.Birthday = new DateTime(1980, 8, 12);
                charityMaker.Rating = 3;
                charityMaker.Avatar = "avatars/seeddata_WillSmith.png";
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

                _unitOfWork.CharityMakers.AddRange(charityMakers);
                _unitOfWork.SaveChangesAsync();
            }

            if (_unitOfWork.BaseItemTypes.Get(v => v.ID == v.ID) != null)
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

                _unitOfWork.BaseItemTypes.AddRange(baseItemTypes);
                _unitOfWork.SaveChangesAsync();
            }
        }
    }
}