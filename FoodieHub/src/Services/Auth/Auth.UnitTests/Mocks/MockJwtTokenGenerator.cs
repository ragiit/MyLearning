namespace Auth.UnitTests.Mocks
{
    public static class MockJwtTokenGenerator
    {
        public static Mock<IJwtTokenGenerator> GetMockJwtTokenGenerator()
        {
            var generator = new Mock<IJwtTokenGenerator>();

            generator.Setup(g => g.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                     .Returns("mocked-jwt-token");
            generator.Setup(g => g.GenerateRefreshToken())
                     .Returns("mocked-refresh-token");

            return generator;
        }
    }
}