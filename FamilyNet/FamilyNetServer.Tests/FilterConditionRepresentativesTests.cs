using FamilyNetServer.Filters;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyNetServer.Tests
{
    class FilterConditionRepresentativesTests
    {
        private IFilterConditionsRepresentatives _filter;
        private Representative _representative1;
        private Representative _representative2;
        private Representative _representative3;
        private Representative _representative4;
        private Representative _representative5;

        private IQueryable<Representative> _representatives;
        private IQueryable<Representative> _emptyList;

        [SetUp]
        public void SetUp()
        {
            _representative1 = new Representative()
            {
                FullName = new FullName()
                {
                    Name = "Иван",
                    Surname = "Иванов",
                    Patronymic = "Иванович"
                },
                Rating = 6.3f,
                Birthday = new DateTime(1990, 6, 3),
                OrphanageID = 1
            };

            _representative2 = new Representative()
            {
                FullName = new FullName()
                {
                    Name = "Петр",
                    Surname = "Петров",
                    Patronymic = "Петрович"
                },
                Rating = 7.2f,
                Birthday = new DateTime(1885, 7, 8),
                OrphanageID = 2
            };

            _representative3 = new Representative()
            {
                FullName = new FullName()
                {
                    Name = "Макисм",
                    Surname = "Максимов",
                    Patronymic = "Максимович"
                },
                Rating = 4.1f,
                Birthday = new DateTime(1991, 3, 4),
                OrphanageID = 1
            };

            _representative4 = new Representative()
            {
                FullName = new FullName()
                {
                    Name = "Николай",
                    Surname = "Николаев",
                    Patronymic = "Николаевич"
                },
                Rating = 4.5f,
                Birthday = new DateTime(1988, 12, 12),
                OrphanageID = 2
            };

            _representative5 = new Representative()
            {
                FullName = new FullName()
                {
                    Name = "Александр",
                    Surname = "Александров",
                    Patronymic = "Александрович"
                },
                Rating = 3.2f,
                Birthday = new DateTime(1954, 5, 8),
                OrphanageID = 2
            };

            _representatives = new List<Representative>()
            {
                _representative1,
                _representative2,
                _representative3,
                _representative4,
                _representative5
            }.AsQueryable();

            _emptyList = new List<Representative>().AsQueryable();



            _filter = new FilterConditionsRepresentatives();
        }

        [Test]
        public void GetRepresentatives_WithEmptyFilter_ShouldReturnCollectionOfAll()
        {
            //Act
            var result = _filter.GetRepresentatives(_representatives, null);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithNullCollection_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иван"
            };

            //Act
            var result = _filter.GetRepresentatives(null, conditions);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetRepresentatives_WithUnexistingName_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "John"
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }

        [Test]
        public void GetRepresentatives_WithExistingName_ShouldReturnOneRepresentativeContatinsCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иван"
            };
            var expectedList = new List<Representative>()
            { 
                _representative1
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }
        
        [Test]
        public void GetRepresentatives_WithUnexistingSurname_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Smith"
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }

        [Test]
        public void GetRepresentatives_WithExistingSurname_ShouldReturnOneRepresentativeContatinsCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иванов"
            };
            var expectedList = new List<Representative>()
            { 
                _representative1
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }
        
        [Test]
        public void GetRepresentatives_WithUnexistingPatronymic_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Johnson"
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }

        [Test]
        public void GetRepresentatives_WithExistingPatronymic_ShouldReturnOneRepresentativeContatinsCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иванович"
            };
            var expectedList = new List<Representative>()
            { 
                _representative1
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }

        [Test]
        public void GetRepresentatives_WithRating_ShouldReturnBetterRatingRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Rating = 4.1f
            };

            var expectedList1 = new List<Representative>()
            {
                _representative3,
                _representative5
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList1);
        }
        
        [Test]
        public void GetRepresentatives_WithInsufficientRating_ShouldReturnEmptyList()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Rating = 2.0f
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }


    }
}
