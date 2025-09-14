using NUnit.Framework;

public class MenuClassesTests
{
    [Test]
    public void ChooseClass_AssignsCorrectClass()
    {
        // Arrange
        var menu = new MenuClasses();

        // Act
        menu.ChooseClass("Mage");

        // Assert
        Assert.AreEqual("Mage", MenuClasses.SelectedClass);
    }
}