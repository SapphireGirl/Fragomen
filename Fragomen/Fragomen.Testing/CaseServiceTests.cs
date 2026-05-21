using Fragomen.UserAPI.Interfaces;
using Fragomen.UserAPI.Models;
using Fragomen.UserAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fragomen.Testing
{

    [TestFixture]
    public class CaseServiceTests
    {
        private Mock<ICaseRepository> _repositoryMock;
        private Mock<ILogger<CaseService>> _loggerMock;
        private CaseService _sutCaseService;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ICaseRepository>();
            _loggerMock = new Mock<ILogger<CaseService>>();
            _sutCaseService = new CaseService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetCaseDetailsAsync_WhenCaseExists_LogsAndReturnsCase()
        {
            // Arrange
            var fakeCase = new Case
            {
                CaseId = 1,
                CaseNumber = "2026-0001",
                SettlementAmount = 2_000_000M, // triggers high-value branch
                CaseParties = new List<CaseParty> { new CaseParty() }
            };
            _repositoryMock.Setup(r => r.GetCase_PartiesByCaseIdAsync(1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(fakeCase);

            // Act
            var result = await _sutCaseService.GetCaseDetailsAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CaseNumber, Is.EqualTo("2026-0001"));

            // Verify the repository was called with correct arguments
            _repositoryMock.Verify(r => r.GetCase_PartiesByCaseIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);

            // Verify specific log lines (shows Moq spy technique for ILogger)
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieving case with ID")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("High-value case detected")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetCaseDetailsAsync_WhenCaseIsNull_LogsWarning()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetCase_PartiesByCaseIdAsync(2, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Case)null);

            // Act
            var result = await _sutCaseService.GetCaseDetailsAsync(2);

            // Assert
            // Assert
            Assert.That(result, Is.Null);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("No case found for ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetCaseDetailsAsync_GroupsByStateProvinceCorrectly()
        {
            // Arrange
            var waParty1 = new Party { PartyId = 1, StateProvince = "WA" };
            var waParty2 = new Party { PartyId = 2, StateProvince = "WA" };
            var caParty = new Party { PartyId = 3, StateProvince = "CA" };

            var caseParties = new List<CaseParty>
        {
            new CaseParty { Party = waParty1 },
            new CaseParty { Party = waParty2 },
            new CaseParty { Party = caParty },
            new CaseParty { Party = null },                       // should be ignored
            new CaseParty { Party = new Party { StateProvince = "", PartyId = 99 } }, // should be ignored
        };

            var caseObj = new Case { CaseId = 1, CaseParties = caseParties };

            _repositoryMock
                .Setup(r => r.GetCase_PartiesByCaseIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(caseObj);

            // Now construct the SUT
            _sutCaseService = new CaseService(_repositoryMock.Object, _loggerMock.Object);

            // Act
            var result = await _sutCaseService.GetGroupByForCaseDetails(1, CancellationToken.None);
            // Assert

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(g => g.StateProvince == "WA"), Is.True);

            var waGroup = result.FirstOrDefault(g => g.StateProvince == "WA");
            var caGroup = result.FirstOrDefault(g => g.StateProvince == "CA");

            Assert.That(waGroup.Parties.Count, Is.EqualTo(2));
            Assert.That(caGroup.Parties.Count, Is.EqualTo(1));


        }

        [Test]
        public async Task ValidateStatusChangeAsync_ReturnsTrue_Intake_To_Active()
        {
            // validation rules: intake -> active, active -> pending/closed, pending -> active/closed, closed -> no changes
            // Arrange
            var caseId = 3;
            var newStatus = "Active";

            _repositoryMock.Setup(r => r.GetCaseStatus(caseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Case { Status = "intake" });

            // Act
            var validationResult = _sutCaseService.ValidateStatusChangeAsync(caseId, newStatus);

            // Assert
            Assert.That(validationResult.Result, Is.True);
        }

        [Test]
        public async Task ValidateStatusChangeAsync_ReturnsFalse_Intake_To_Closed()
        {
            // arrange
            var caseId = 1;
            var newStatus = "Closed";

            _repositoryMock.Setup(r => r.GetCaseStatus(caseId, It.IsAny <CancellationToken>()))
                .ReturnsAsync(new Case { Status = "Intake" });

            // act
            // System Under Test: CaseService.ValidateStatusChangeAsync
            var validationResult = _sutCaseService.ValidateStatusChangeAsync(caseId, newStatus);

            // Assert
            Assert.That(validationResult.Result, Is.False);
        }
    }
}