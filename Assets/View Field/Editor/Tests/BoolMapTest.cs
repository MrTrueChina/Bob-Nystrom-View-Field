using NUnit.Framework;
using UnityEngine;

namespace MtC.Tools.FoV
{
    [TestFixture]
    public class ViewFieldTest
    {
        const int WIDTH = 8;
        const int HEIGHT = 5;
        BoolMap _viewField;

        [SetUp]
        public void SetUp()
        {
            _viewField = new ViewFieldTestSub(WIDTH, HEIGHT);
        }

        [TearDown]
        public void TearDown()
        {
            _viewField = null;
        }

        [Test]
        [TestCase(WIDTH - 1, HEIGHT - 1)]
        [TestCase(WIDTH - 1, 0)]
        [TestCase(0, 0)]
        [TestCase(0, HEIGHT - 1)]
        public void Contains_Contains(int x, int y)
        {
            Assert.IsTrue(_viewField.Contains(new Vector2(x, y)));
        }

        [Test]
        [TestCase(0, HEIGHT)]
        [TestCase(WIDTH, 0)]
        [TestCase(0, -1)]
        [TestCase(-1, 0)]
        public void Contains_NotContains(int x, int y)
        {
            Assert.IsFalse(_viewField.Contains(new Vector2(x, y)));
        }
    }

    class ViewFieldTestSub : BoolMap
    {
        public ViewFieldTestSub(int width, int height) : base(width, height)
        {

        }
    }
}
