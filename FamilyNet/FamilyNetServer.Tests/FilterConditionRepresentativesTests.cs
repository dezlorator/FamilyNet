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
                Birthday = new DateTime(1985, 7, 8),
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
                OrphanageID = 3
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
        public void GetRepresentatives_WithNullName_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = null
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
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
        public void GetRepresentatives_WithNullSurname_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = null
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
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
        public void GetRepresentatives_WithNullPatronymic_ShouldReturnNull()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = null
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
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

            var expectedList = new List<Representative>()
            {
                _representative3,
                _representative5
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
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

        [Test]
        public void GetRepresentatives_WithChildrenHouseID_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                ChildrenHouseID = 2
            };

            var expectedList = new List<Representative>()
            {
                _representative2,
                _representative4
            }.AsQueryable();


            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }

        [Test]
        public void GetRepresentatives_WithZeroChildrenHouseID_ShouldReturnAllRepresentativesList()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                ChildrenHouseID = 0
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithAge_ShouldReturnRepresentativesList()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Age = DateTime.Now.Year - new DateTime(1985, 1, 1).Year
            };

            var expectedList = new List<Representative>()
            {
                _representative2,
                _representative5
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }

        [Test]
        public void GetRepresentatives_WithBigAge_ShouldReturnEmptyRepresentativesList()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Age = DateTime.Now.Year - new DateTime(1885, 1, 1).Year
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }

        [Test]
        public void GetRepresentatives_WithInvalidAge_ShouldReturnAllRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Age = DateTime.Now.Year - (DateTime.Now.Year - 1)
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithRows_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Rows = 2
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithPage_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Page = 2
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithRowsAndPage_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Rows = 2,
                Page = 2
            };

            var expectedList = new List<Representative>()
            {
                _representative3,
                _representative4
            }.AsQueryable();

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, expectedList);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-2, -1)]
        [TestCase(-2, 1)]
        [TestCase(0, -1)]
        [TestCase(2, 0)]
        [TestCase(2, -1)]
        public void GetRepresentatives_WithIvalidRowsAndPage_ShouldReturnRepresentativesCollection(int page, int rows)
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Rows = rows,
                Page = page
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _representatives);
        }

        [Test]
        public void GetRepresentatives_WithAllParameters_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иван",
                Rating = 6.3f,
                ChildrenHouseID = 1,
                Age = DateTime.Now.Year - new DateTime(1990, 1, 1).Year,
                Rows = 3,
                Page = 1
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
        public void GetRepresentatives_WithAllParametersAndBigPage_ShouldReturnRepresentativesCollection()
        {
            //Arrange
            var conditions = new FilterParametersRepresentatives()
            {
                Name = "Иван",
                Rating = 6.3f,
                ChildrenHouseID = 1,
                Age = DateTime.Now.Year - new DateTime(1990, 1, 1).Year,
                Rows = 3,
                Page = 10
            };

            //Act
            var result = _filter.GetRepresentatives(_representatives, conditions);

            //Assert
            Assert.AreEqual(result, _emptyList);
        }


    }
}
