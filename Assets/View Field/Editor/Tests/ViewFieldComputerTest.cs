using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace MtC.Tools.FoV
{
    [TestFixture]
    public class ViewFieldComputerTest
    {
        [Test]
        public void Compute_Normal()
        {
            ViewField viewField = ViewFieldComputer.Compute(new VisibleMap(10, 10), new Vector2(5, 5));
        }
    }
}
