﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHibernate.Caches.Redis.Tests
{
    public class PerformanceTests : IntegrationTestBase
    {
        [Fact]
        async Task concurrent_sessions()
        {
            DisableLogging();

            const int iterations = 10000;
            var sessionFactory = CreateSessionFactory();

            var tasks = Enumerable.Range(0, iterations).Select(i =>
            {
                return Task.Run(() =>
                {
                    object entityId = null;
                    UsingSession(sessionFactory, session =>
                    {
                        var entity = new Person("Foo", 1);
                        entityId = session.Save(entity);
                    });

                    UsingSession(sessionFactory, session =>
                    {
                        var entity = session.Load<Person>(entityId);
                        Assert.NotNull(entity);
                    });
                });
            });

            await Task.WhenAll(tasks);
        }
    }
}
