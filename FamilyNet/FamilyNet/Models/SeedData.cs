using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;


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

                #region Volunteer1

                Volunteer volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Ганна", Surname = "Бутенко", Patronymic = "Андріївна" };
                volunteer.Birthday = new DateTime(1998, 11, 30);
                volunteer.Rating = 8;
                volunteer.Avatar = "ewwwee";
                volunteer.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Дніпропетровська",
                    City = "Новомосковськ",
                    Street = "Шкільна",
                    House = "4"
                };

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer2

                volunteer = new Volunteer();
                volunteer.FullName = new FullName(){ Name = "Іван", Surname = "Новіков", Patronymic = "Русланович" };
                volunteer.Birthday = new DateTime(1991, 4, 10);
                volunteer.Rating = 3;
                volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Дніпропетровська",
                    City = "Дніпро",
                    Street = "Козацька",
                    House = "14"
                };

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer3

                volunteer = new Volunteer();
                
                   volunteer.FullName = new FullName() { Name = "Михайло", Surname = "Іванов", Patronymic = "Сергійович" };
                   volunteer.Birthday = new DateTime(1993, 12, 25);
                   volunteer.Rating = 4;
                   volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Київська",
                    City = "Київ",
                    Street = "Героїв",
                    House = "111"
                };

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer4

                volunteer = new Volunteer();
                    volunteer.FullName = new FullName() { Name = "Марина", Surname = "Романчук", Patronymic = "Сергіївна" };
                    volunteer.Birthday = new DateTime(1999, 1, 20);
                    volunteer.Rating = 7;
                    volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Дніпропетровська",
                    City = "Дніпро",
                    Street = "Гагаріна",
                    House = "63"
                };

                #endregion

                volunteers.Add(volunteer);

                #region Volunteer5

                volunteer = new Volunteer();
                volunteer.FullName = new FullName() { Name = "Аліна", Surname = "Лейко", Patronymic = "Олегівна" };
                volunteer.Birthday = new DateTime(1985, 4, 1);
                volunteer.Rating = 10;
                volunteer.Avatar = "";
                volunteer.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Київська",
                    City = "Київ",
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
                    Country = "Україна",
                    Region = "Херсонська",
                    City = "Херсон",
                    Street = "Молодіжна",
                    House = "12"
                };
                orphanage.Location = new Location() {
                    MapCoordX = 46.64748f,
                    MapCoordY = 32.61608f
                };
                orphanage.Rating = 11;
                orphanage.Avatar = "3nf3pby0.jpg";

                List<Representative> representatives = new List<Representative>();

                Representative representative = new Representative();
                
                representative.FullName = new FullName() { Name = "Олег", Surname = "Петренко", Patronymic = "Дмитрович" };
                representative.Birthday = new DateTime(1954, 12, 30);
                representative.Rating = 5;
                representative.Avatar = "";
                
                representatives.Add(representative);
                orphanage.Representatives = representatives;

                List<Orphan> orphans = new List<Orphan>();

                Orphan orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Гліб", Surname = "Левада", Patronymic = "Русланович" };
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

                orphanage.Name = "Ельф";
                orphanage.Adress = new Address()
                {
                    Country = "Україна",
                    Region = "Сумська",
                    City = "Суми",
                    Street = "Котляревського",
                    House = "89"
                };
                orphanage.Location = new Location() 
                {
                    MapCoordX=50.895342f,
                    MapCoordY=34.751811f
                };
                orphanage.Rating = 7;
                orphanage.Avatar = "jltx4rd2.jpg";

                representatives = new List<Representative>();
                representative = new Representative();

                representative.FullName = new FullName() { Name = "Марина", Surname = "Кричич", Patronymic = "Михайлівна" };
                representative.Birthday = new DateTime(1954, 12, 30);
                representative.Rating = 5;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName(){Name = "Анастасія", Surname = "Горб", Patronymic = "Андріївна" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);


                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Марія", Surname = "Павленко", Patronymic = "Олександрівна" };
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
                    Country = "Україна",
                    Region = "Дніпропетрровська",
                    City = "Нікополь",
                    Street = "Херсонська",
                    House = "177"
                };
                orphanage.Location = new Location() 
                {
                    MapCoordX=47.563908f,
                    MapCoordY=34.378792f
                };
                orphanage.Rating = 8;
                orphanage.Avatar = "proeundk.jpg";

                representative = new Representative();
                representatives = new List<Representative>();

                representative.FullName = new FullName() { Name = "Гліб", Surname = "Голосенін", Patronymic = "Іванович" };
                representative.Birthday = new DateTime(1988, 3, 4);
                representative.Rating = 5;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;


                orphans = new List<Orphan>();
                orphan = new Orphan();

                
                orphan.FullName= new FullName(){Name = "Арсен", Surname = "Зубарян", Patronymic = "Кирилович" };
                orphan.Birthday = new DateTime(2004, 5, 14);
                orphan.Rating = 8;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Віталій", Surname = "Цаль", Patronymic = "Ільіч" };
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

                orphanage.Name = "Сонечко";
                orphanage.Adress = new Address()
                {
                    Country = "Україна",
                    Region = "Київська",
                    City = "Біла Церква",
                    Street = "Гаек",
                    House = "227"
                };
                orphanage.Location = new Location() 
                {
                    MapCoordX=50.275774f,
                    MapCoordY=29.873140f
                };
                orphanage.Rating = 3;
                orphanage.Avatar = "rwlmqppn.jpg";

                representative = new Representative();
                representatives = new List<Representative>();


                representative.FullName = new FullName() { Name = "Спартак", Surname = "Олексій", Patronymic = "Андрійович" };
                representative.Birthday = new DateTime(1988, 3, 4);
                representative.Rating = 8;
                representative.Avatar = "";

                representatives.Add(representative);
                orphanage.Representatives = representatives;

                orphans = new List<Orphan>();
                orphan = new Orphan();

                orphan.FullName = new FullName() { Name = "Захар", Surname = "Мисра", Patronymic = "Володимирович" };
                orphan.Birthday = new DateTime(2011, 11, 14);
                orphan.Rating = 7;
                orphan.Avatar = "";
                orphan.ChildInOrphanage = true;
                orphan.Confirmation = true;

                orphans.Add(orphan);

                orphan = new Orphan();


                orphan.FullName = new FullName() { Name = "Ганна", Surname = "Анісімова", Patronymic = "Сергіївна" };
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

                charityMaker.FullName = new FullName() { Name = "Володимир", Surname = "Халін", Patronymic = "Тарасович" };
                charityMaker.Birthday = new DateTime(1991, 12, 3);
                charityMaker.Rating = 5;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Харьковская",
                    City = "Харьков",
                    Street = "Титова",
                    House = "22"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker2

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Сергій", Surname = "Калініч", Patronymic = "Євгенович" };
                charityMaker.Birthday = new DateTime(1972, 6, 12);
                charityMaker.Rating = 7;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Львівська",
                    City = "Львів",
                    Street = "Наукова",
                    House = "15"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker3

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Альона", Surname = "Чіпко", Patronymic = "Максимівна" };
                charityMaker.Birthday = new DateTime(1993, 4, 28);
                charityMaker.Rating = 9;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Донецька",
                    City = "Донецьк",
                    Street = "Семашко",
                    House = "86"
                };
                #endregion

                charityMakers.Add(charityMaker);

                #region CharityMaker4

                charityMaker = new CharityMaker();

                charityMaker.FullName = new FullName() { Name = "Христина", Surname = "Юдіна", Patronymic = "Сергіївна" };
                charityMaker.Birthday = new DateTime(1984, 5, 18);
                charityMaker.Rating = 10;
                charityMaker.Avatar = "";
                charityMaker.Address = new Address()
                {
                    Country = "Україна",
                    Region = "Одеська",
                    City = "Одеса",
                    Street = "Лютеранський Перевулок",
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

                baseItemType.Name = "Взуття";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Верхній одяг";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Брюки";
                baseItemTypes.Add(baseItemType);

                baseItemType = new DonationItemType();
                baseItemType.Name = "Футболки";
                baseItemTypes.Add(baseItemType);

                _unitOfWorkAsync.BaseItemTypes.AddRange(baseItemTypes);
                _unitOfWorkAsync.SaveChangesAsync();
            }
        }
    }
}
