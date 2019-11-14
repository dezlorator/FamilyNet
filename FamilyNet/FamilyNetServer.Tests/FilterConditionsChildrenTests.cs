using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;


namespace FamilyNetServer.Tests
{
    class FilterConditionsChildrenTests
    {
        private IFilterConditionsChildren _filter;

        [SetUp]
        public void Init()
        {
            var mockLogger = new Mock<ILogger<FilterConditionsChildren>>();
            _filter = new FilterConditionsChildren(mockLogger.Object);
        }

        private static IQueryable<Orphan> ChildrenCollection()
        {
            var child1 = new Orphan()
            {
                FullName = new FullName()
                {
                    Name = "Тимофей",
                    Patronymic = "Михайлович",
                    Surname = "Лепетя"
                },
                Rating = 5.5f,
                Birthday = new DateTime(2018, 5, 5),
                OrphanageID = 1
            };

            var child2 = new Orphan()
            {
                FullName = new FullName()
                {
                    Name = "Виталий",
                    Patronymic = "Михайлович",
                    Surname = "Лепетя"
                },
                Rating = 2.5f,
                Birthday = new DateTime(2016, 5, 20),
                OrphanageID = 2
            };

            var child3 = new Orphan()
            {
                FullName = new FullName()
                {
                    Name = "Кирилл",
                    Patronymic = "Иванович",
                    Surname = "Титов"
                },
                Rating = 4.8f,
                Birthday = new DateTime(2011, 3, 8),
                OrphanageID = 1
            };

            var child4 = new Orphan()
            {
                FullName = new FullName()
                {
                    Name = "Елена",
                    Patronymic = "Юрьевна",
                    Surname = "Завада"
                },
                Rating = 3.8f,
                Birthday = new DateTime(2013, 11, 21),
                OrphanageID = 2
            };

            var child5 = new Orphan()
            {
                FullName = new FullName()
                {
                    Name = "Александра",
                    Patronymic = "Артуровна",
                    Surname = "Семенова"
                },
                Rating = 2.2f,
                Birthday = new DateTime(2016, 1, 1),
                OrphanageID = 2
            };

            return new List<Orphan>() { child1, child2, child3, child4, child5 }.AsQueryable();
        }

        [Test]
        public void FilterConditionsChildren_WithNullParameters_ShouldReturnFullCollection()
        {
            var children = ChildrenCollection();

            var newCollection = _filter.GetOrphans(children, null);

            Assert.AreEqual(newCollection, children);
        }

        [Test]
        public void FilterConditionsChildren_WithoutEmptyParameters_ShouldReturnFullCollection()
        {
            var children = ChildrenCollection();

            var newCollection = _filter.GetOrphans(children, new FilterParemetersChildren());

            Assert.AreEqual(newCollection, children);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterAbsentNameParameters_ShouldReturnEmptyCollection()
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Name = "Мирина"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [Test]
        public void FilterConditionsChildren_WithFilterExistsNameParameters_ShouldReturnNewCollection()
        {
            var name = "Тимофей";
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Name = name
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Where(c => c.FullName
                .ToString().Contains(name));

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterAbsentSurnameParameters_ShouldReturnEmptyCollection()
        {
            var children = ChildrenCollection();
            var name = "Ерисов";

            var parameters = new FilterParemetersChildren()
            {
                Name = name
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Where(c => c.FullName.ToString()
                .Contains(name));

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterExistsSurnameParameters_ShouldReturnNewCollection()
        {
            var children = ChildrenCollection();
            var name = "Лепетя";

            var parameters = new FilterParemetersChildren()
            {
                Name = name
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Where(c => c.FullName
                .ToString().Contains(name));

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [Test]
        public void FilterConditionsChildren_WithFilterAbsentPatronymicParameters_ShouldReturnEmptyCollection()
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Name = "Степановна"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterExistsPatronymicParameters_ShouldReturnNewCollection()
        {
            var children = ChildrenCollection();
            var name = "Михайлович";

            var parameters = new FilterParemetersChildren()
            {
                Name = name
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Where(c => c.FullName.ToString()
                .Contains(name));

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterRatingParameters_ShouldReturnNewCollection()
        {
            var children = ChildrenCollection();
            var rating = 2.6f;

            var parameters = new FilterParemetersChildren()
            {
                Rating = rating
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Where(c => c.Rating >= rating);

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterPageRowParameters_ShouldReturnNewCollection()
        {
            var children = ChildrenCollection();
            var page = 1;
            var rows = 3;

            var parameters = new FilterParemetersChildren()
            {
                Page = page,
                Rows = rows
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children.Skip((page - 1) * rows).Take(rows);

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [TestCase(0, 0)]
        [TestCase(-2, -1)]
        [TestCase(-2, 1)]
        [TestCase(0, -1)]
        [TestCase(2, 0)]
        public void FilterConditionsChildren_WithFilterInvalidPageRowParameters_ShouldReturnSameCollection(int page, int rows)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Page = page,
                Rows = rows
            };

            var newCollection = _filter.GetOrphans(children, parameters).ToList();

            Assert.AreEqual(newCollection, children);
        }

        [TestCase(1)]
        public void FilterConditionsChildren_WithFilterChildrenHouseParameters_ShouldReturnNewCollection(int houseId)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                ChildrenHouseID = houseId
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children
                .Where(c => c.OrphanageID == parameters.ChildrenHouseID)
                .AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        public static IEnumerable<int> AgeRange = Enumerable.Range(0, 18);


        [Test, TestCaseSource("AgeRange")]
        public void FilterConditionsChildren_WithFilterAgeParameters_ShouldReturnNewCollection(int age)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Age = age
            };

            var daysPerYear = 366;
            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children
                .Where(c => (DateTime.Now - c.Birthday).Days > parameters.Age * daysPerYear)
                .AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [TestCase(2, "Лепетя")]
        [TestCase(5, "Семенова")]
        public void FilterConditionsChildren_WithFilterAgeNameParameters_ShouldReturnNewCollection(int age, string name)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Age = age,
                Name = name
            };

            var daysPerYear = 366;
            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = children
                .Where(c => (DateTime.Now - c.Birthday).Days > parameters.Age * daysPerYear
                && c.FullName.ToString().Contains(parameters.Name))
                .AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [TestCase(2, null, -3.0f)]
        [TestCase(0, null, -3.0f)]
        [TestCase(2, "Лепетя", 3.0f)]
        [TestCase(5, "Семенова", 10.0f)]
        public void FilterConditionsChildren_WithFilterAgeNameRatingParameters_ShouldReturnNewCollection(int age, string name, float rating)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Age = age,
                Name = name,
                Rating = rating
            };

            var newCollection = _filter.GetOrphans(children, parameters);

            var daysPerYear = 366;
            var expectedCollection = children
                .Where(c => (DateTime.Now - c.Birthday).Days > parameters.Age * daysPerYear
                && c.Rating > parameters.Rating)
                .AsQueryable();

            if (!String.IsNullOrEmpty(parameters.Name))
            {
                expectedCollection = expectedCollection.Where(c => c.FullName.ToString().Contains(parameters.Name));
            }

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [TestCase(2, null, -3.0f, -1)]
        [TestCase(2, null, -3.0f, 1)]
        [TestCase(0, null, -3.0f, 1)]
        [TestCase(2, "Лепетя", 3.0f, 2)]
        [TestCase(5, "Семенова", 10.0f, 5)]
        public void FilterConditionsChildren_WithFilterAgeNameRatingChildrenHouseIdParameters_ShouldReturnNewCollection(int age,
            string name, float rating, int childrenHouseId)
        {
            var children = ChildrenCollection();

            var parameters = new FilterParemetersChildren()
            {
                Age = age,
                Name = name,
                Rating = rating,
                ChildrenHouseID = childrenHouseId
            };

            var newCollection = _filter.GetOrphans(children, parameters);

            var daysPerYear = 366;
            var expectedCollection = children
                .Where(c => (DateTime.Now - c.Birthday).Days > parameters.Age * daysPerYear
                && c.Rating > parameters.Rating)
                .AsQueryable();

            if (parameters.ChildrenHouseID > 0)
            {
                expectedCollection = expectedCollection
                .Where(c => c.OrphanageID == parameters.ChildrenHouseID)
                .AsQueryable();
            }

            if (!String.IsNullOrEmpty(parameters.Name))
            {
                expectedCollection = expectedCollection.Where(c => c.FullName.ToString().Contains(parameters.Name));
            }

            Assert.AreEqual(expectedCollection, newCollection);
        }
    }
}