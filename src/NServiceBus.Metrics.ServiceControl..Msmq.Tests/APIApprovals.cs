﻿using System.Runtime.CompilerServices;
using NServiceBus;
using NUnit.Framework;
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{
    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Approve()
    {
        var publicApi = ApiGenerator.GeneratePublicApi(typeof(MetricsOptionsExtensions).Assembly);
        TestApprover.Verify(publicApi);
    }
}