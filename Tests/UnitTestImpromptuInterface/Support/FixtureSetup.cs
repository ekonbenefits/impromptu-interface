﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.Build;


namespace UnitTestImpromptuInterface
{
#if !SILVERLIGHT && !SELFRUNNER && !NETCOREAPP2_0 && false

    using NUnit.Framework;
    [SetUpFixture]
    public class FixtureSetup
    {
        private IDisposable Builder;
        
        [OneTimeSetUp]
        public void Setup()
        {
            Builder = BuildProxy.WriteOutDll("ImpromptuEmit");
        }
        [OneTimeTearDown]
        public void TearDown()
        {
            Builder.Dispose();
        }
    }
#endif
}
