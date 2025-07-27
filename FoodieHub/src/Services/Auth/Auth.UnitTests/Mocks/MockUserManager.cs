namespace Auth.UnitTests.Mocks
{
    public static class MockUserManager
    {
        public static Mock<UserManager<TUser>> GetMockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup default behavior for common methods
            userManager.Setup(m => m.CheckPasswordAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                       .ReturnsAsync(true);
            userManager.Setup(m => m.GetRolesAsync(It.IsAny<TUser>()))
                       .ReturnsAsync(new List<string> { "Customer" });
            userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                       .ReturnsAsync((TUser)null); // Default: user not found

            return userManager;
        }
    }
}