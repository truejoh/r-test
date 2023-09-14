using System;
using Microsoft.AspNetCore.Mvc;
using Recievables.Controllers;
using Recievables.Models;
using Recievables.Repositories;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Recievables.Data;

namespace Recievables.Tests
{
    public class ReceivablesControllerTests
    {
        private readonly ReceivablesController _controller;
        private readonly Mock<IReceivablesRepository> _repository;

        public ReceivablesControllerTests()
        {
            _repository = new Mock<IReceivablesRepository>();
            _controller = new ReceivablesController(_repository.Object);
        }

        [Fact]
        public async Task AddReceivables_ValidPayload_ReturnsOkResult()
        {
            // Arrange
            var payload = new ReceivablePayload
            {
                Receivables = new List<Receivable>
                {
                    new Receivable { Reference = "ref1" },
                    new Receivable { Reference = "ref2" },
                }
            };

            // Act
            var result = await _controller.AddReceivables(payload) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var ResultPayload = result.Value as List<Receivable>;
            Assert.NotNull(ResultPayload);
            Assert.Equal(payload.Receivables.Count, ResultPayload.Count);
        }

        [Fact]
        public async Task GetSummary_ReturnsSummaryOkResult()
        {
            // Arrange
            _repository.Setup(repo => repo.GetSummaryAsync())
                .ReturnsAsync(new ReceivablesSummary { OpenInvoicesValue = 0.0m, ClosedInvoicesValue = 0.0m });

            // Act
            var result = await _controller.GetSummary() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var summary = result.Value as ReceivablesSummary;
            Assert.NotNull(summary);
            Assert.Equal(0.0m, summary.OpenInvoicesValue);
            Assert.Equal(0.0m, summary.ClosedInvoicesValue);
        }

        [Fact]
        public async Task GetSummary_ReturnsCorrectSummary()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var receivables = new List<Receivable>
                {
                    new Receivable { Reference = "ref1", OpeningValue = 1000.0m, PaidValue = 0.0m, Cancelled = false },
                    new Receivable { Reference = "ref2", OpeningValue = 0.0m, PaidValue = 500.0m, Cancelled = false, ClosedDate = "2023-07-15" },
                    new Receivable { Reference = "ref3", OpeningValue = 2000.0m, PaidValue = 1000.0m, Cancelled = false, ClosedDate = "2023-08-01" },
                    new Receivable { Reference = "ref4", OpeningValue = 0.0m, PaidValue = 750.0m, Cancelled = true, ClosedDate = "2023-09-01" },
                };

                context.Receivables.AddRange(receivables);
                context.SaveChanges();

                var repository = new ReceivablesRepository(context);
                var controller = new ReceivablesController(repository);

                // Act
                var result = await controller.GetSummary() as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2000.0m, (result.Value as ReceivablesSummary).OpenInvoicesValue);
                Assert.Equal(1500.0m, (result.Value as ReceivablesSummary).ClosedInvoicesValue);
            }
        }

    }
}

