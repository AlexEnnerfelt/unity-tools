using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnpopularOpinion.Tools;

public class WeightedListTests {
    [Test]
    public void WeightedList_PercentageTest() {
        // Arrange
        var weightedList = new WeightedList<string>();

        var listItems = new Dictionary<string, int>() {
            { "Item1", 1},
            { "Item2", 2},
            { "Item3", 5},
            { "Item4", 10},
            { "Item5", 20},
            { "Item6", 28},
            { "Item7", 33},
            { "Item8", 44},
            { "Item9", 55},
            { "Item10", 9},
        };

        var weightedItems = listItems.Select(item => new WeightedListItem<string>(item.Key, item.Value)).ToList();

        weightedList.Add(weightedItems);

        var itemCounts = new Dictionary<string, int>();

        foreach (var item in listItems) {
            itemCounts[item.Key] = 0;
        }


        var totalRetrievals = 10000;

        // Act
        for (var i = 0; i < totalRetrievals; i++) {
            var item = weightedList.Next();
            itemCounts[item]++;
        }

        foreach (var (item, count) in itemCounts) {
            // Assert
            var tolerance = 0.02f; // 2% tolerance
            var expectedPercentage = (float)listItems[item] / (float)weightedList.TotalWeight;

            var actualPercentage = (float)count / totalRetrievals;

            Assert.IsTrue(Math.Abs(actualPercentage - expectedPercentage) <= tolerance,
                $"Expected percentage: {expectedPercentage:P}, Actual percentage: {actualPercentage:P}");
        }
    }
}
