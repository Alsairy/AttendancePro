using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace AttendancePlatform.Tests.Unit.Services
{
    public class CacheServiceTests
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<ILogger<CacheService>> _loggerMock;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _distributedCacheMock = new Mock<IDistributedCache>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<CacheService>>();
            _cacheService = new CacheService(_distributedCacheMock.Object, _memoryCacheMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_WhenMemoryCacheHit_ReturnsFromMemoryCache()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestObject { Id = 1, Name = "Test" };
            object cachedValue = expectedValue;

            _memoryCacheMock.Setup(m => m.TryGetValue(key, out cachedValue))
                          .Returns(true);

            // Act
            var result = await _cacheService.GetAsync<TestObject>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result.Id);
            Assert.Equal(expectedValue.Name, result.Name);
            _distributedCacheMock.Verify(d => d.GetStringAsync(It.IsAny<string>(), default), Times.Never);
        }

        [Fact]
        public async Task GetAsync_WhenMemoryCacheMissButDistributedCacheHit_ReturnsFromDistributedCache()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestObject { Id = 1, Name = "Test" };
            var serializedValue = JsonSerializer.Serialize(expectedValue);
            object cachedValue = null;

            _memoryCacheMock.Setup(m => m.TryGetValue(key, out cachedValue))
                          .Returns(false);
            _distributedCacheMock.Setup(d => d.GetStringAsync(key, default))
                               .ReturnsAsync(serializedValue);

            // Act
            var result = await _cacheService.GetAsync<TestObject>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result.Id);
            Assert.Equal(expectedValue.Name, result.Name);
            _memoryCacheMock.Verify(m => m.Set(key, It.IsAny<TestObject>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_SetsInBothCaches()
        {
            // Arrange
            var key = "test-key";
            var value = new TestObject { Id = 1, Name = "Test" };
            var expiration = TimeSpan.FromMinutes(10);

            // Act
            await _cacheService.SetAsync(key, value, expiration);

            // Assert
            _distributedCacheMock.Verify(d => d.SetStringAsync(
                key, 
                It.IsAny<string>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                default), Times.Once);
            _memoryCacheMock.Verify(m => m.Set(key, value, expiration), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_RemovesFromBothCaches()
        {
            // Arrange
            var key = "test-key";

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            _distributedCacheMock.Verify(d => d.RemoveAsync(key, default), Times.Once);
            _memoryCacheMock.Verify(m => m.Remove(key), Times.Once);
        }

        [Fact]
        public async Task GetOrSetAsync_WhenCacheMiss_CallsGetItemAndCachesResult()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestObject { Id = 1, Name = "Test" };
            object cachedValue = null;

            _memoryCacheMock.Setup(m => m.TryGetValue(key, out cachedValue))
                          .Returns(false);
            _distributedCacheMock.Setup(d => d.GetStringAsync(key, default))
                               .ReturnsAsync((string)null);

            // Act
            var result = await _cacheService.GetOrSetAsync(key, () => Task.FromResult(expectedValue));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue.Id, result.Id);
            _distributedCacheMock.Verify(d => d.SetStringAsync(
                key, 
                It.IsAny<string>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                default), Times.Once);
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
