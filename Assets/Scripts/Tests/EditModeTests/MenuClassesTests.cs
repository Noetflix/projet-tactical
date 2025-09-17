using NUnit.Framework;
using UnityEngine;

public class MenuClassesTests
{
    [Test]
    public void ChooseClass_AssignsCorrectClass()
    {
        // Arrange
        var menu = new MenuClasses();

        // Act
        menu.ChooseClass("Mage");
        Debug.Log($"MenuClasses.SelectedClass is now: {MenuClasses.SelectedClass}");

        // Assert
        Assert.AreEqual("Mage", MenuClasses.SelectedClass);
    }
}