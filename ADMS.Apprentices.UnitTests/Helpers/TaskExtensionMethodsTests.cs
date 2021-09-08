
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Adms.Shared.Testing;
using ADMS.Apprentices.Core.Services.Validators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ADMS.Apprentices.UnitTests.Helpers
{
    [TestClass]
    public class ExceptionsThrownFromTasks : GivenWhenThen
    {
        protected List<Task> tasks;
        protected override void Given()
        {
            base.Given();
            tasks = new List<Task>();
        }

        protected override void When()
        {
            tasks.Add(SimpleTaskWithDelay(new TimeSpan(0,0,0,0,10)));
            tasks.Add(SimpleTaskWithDelayAndError(new TimeSpan(0,0,0,0,20)));
            tasks.Add(SimpleTaskWithDelay(new TimeSpan(0,0,0,0,10)));
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ThenAsyncWillThrowTheExceptionThatOccurredInATask()
        {
            await tasks.WaitAndThrowAnyExceptionFound();
        }
        
        protected async Task SimpleTaskWithDelay(TimeSpan timeSpan)
        {
            await Task.Delay(timeSpan);
        }
        
        protected async Task SimpleTaskWithDelayAndError(TimeSpan timeSpan)
        {
            await Task.Delay(timeSpan);
            throw new Exception("Any Exception");
        }
    }
}