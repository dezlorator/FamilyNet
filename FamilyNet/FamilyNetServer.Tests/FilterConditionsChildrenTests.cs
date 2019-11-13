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
        private Orphan child1;
        private Orphan child2;
        private Orphan child3;
        private Orphan child4;
        private Orphan child5;

        private IQueryable<Orphan> children;

        [SetUp]
        public void Init()
        {
            child1 = new Orphan()
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

            child2 = new Orphan()
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

            child3 = new Orphan()
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

            child4 = new Orphan()
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

            child5 = new Orphan()
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

            children = new List<Orphan>()
            {
                child1, child2, child3, child4, child5
            }.AsQueryable();

            var mockLogger = new Mock<ILogger<FilterConditionsChildren>>();
            _filter = new FilterConditionsChildren(mockLogger.Object);
        }

        [Test]
        public void FilterConditionsChildren_WithNullParameters_ShouldReturnFullCollection()
        {
            var newCollection = _filter.GetOrphans(children, null);

            Assert.AreEqual(newCollection, children);
        }


        [Test]
        public void FilterConditionsChildren_WithoutEmptyParameters_ShouldReturnFullCollection()
        {
            var newCollection = _filter.GetOrphans(children, new FilterParemetersChildren());

            Assert.AreEqual(newCollection, children);
        }


        [Test]
        public void FilterConditionsChildren_WithFilterAbsentNameParameters_ShouldReturnEmptyCollection()
        {
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
            var parameters = new FilterParemetersChildren()
            {
                Name = "Тимофей"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { child1 }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterAbsentSurnameParameters_ShouldReturnEmptyCollection()
        {
            var parameters = new FilterParemetersChildren()
            {
                Name = "Ерисов"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterExistsSurnameParameters_ShouldReturnNewCollection()
        {
            var parameters = new FilterParemetersChildren()
            {
                Name = "Лепетя"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { child1, child2 }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }


        [Test]
        public void FilterConditionsChildren_WithFilterAbsentPatronymicParameters_ShouldReturnEmptyCollection()
        {
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
            var parameters = new FilterParemetersChildren()
            {
                Name = "Михайлович"
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { child1, child2 }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterRatingParameters_ShouldReturnNewCollection()
        {
            var parameters = new FilterParemetersChildren()
            {
                Rating = 2.6f
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { child1, child3, child4 }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        public void FilterConditionsChildren_WithFilterPageRowParameters_ShouldReturnNewCollection()
        {
            var parameters = new FilterParemetersChildren()
            {
                Page = 1,
                Rows = 3
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>() { child1, child2, child3 }.AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-2, -1)]
        [TestCase(-2, 1)]
        [TestCase(0, -1)]
        [TestCase(2, 0)]
        public void FilterConditionsChildren_WithFilterInvalidPageRowParameters_ShouldReturnSameCollection(int page, int rows)
        {
            var parameters = new FilterParemetersChildren()
            {
                Page = page,
                Rows = rows
            };

            var newCollection = _filter.GetOrphans(children, parameters);
            var expectedCollection = new List<Orphan>()
                { child1, child2, child3, child4, child5 }
            .AsQueryable();

            Assert.AreEqual(expectedCollection, newCollection);
        }

        [Test]
        [TestCase(1)]
        public void FilterConditionsChildren_WithFilterChildrenHouseParameters_ShouldReturnNewCollection(int houseId)
        {
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

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        public void FilterConditionsChildren_WithFilterAgeParameters_ShouldReturnNewCollection(int age)
        {
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


        [Test]
        [TestCase(2, "Лепетя")]
        [TestCase(5, "Семенова")]
        public void FilterConditionsChildren_WithFilterAgeNameParameters_ShouldReturnNewCollection(int age, string name)
        {
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


        [Test]
        [TestCase(2, null, -3.0f)]
        [TestCase(0, null, -3.0f)]
        [TestCase(2, "Лепетя", 3.0f)]
        [TestCase(5, "Семенова", 10.0f)]
        public void FilterConditionsChildren_WithFilterAgeNameRatingParameters_ShouldReturnNewCollection(int age, string name, float rating)
        {
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


        [Test]
        [TestCase(2, null, -3.0f, -1)]
        [TestCase(2, null, -3.0f, 1)]
        [TestCase(0, null, -3.0f, 1)]
        [TestCase(2, "Лепетя", 3.0f, 2)]
        [TestCase(5, "Семенова", 10.0f, 5)]
        public void FilterConditionsChildren_WithFilterAgeNameRatingChildrenHouseIdParameters_ShouldReturnNewCollection(int age,
            string name, float rating, int childrenHouseId)
        {
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