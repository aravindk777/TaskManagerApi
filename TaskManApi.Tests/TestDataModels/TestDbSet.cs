using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManApi.Data;

namespace TaskManApi.Tests.TestDataModels
{
    internal class TestDbSet : TestDbContext<Models.Task>
    {
        public TestDbSet()
        {
        }

        public override Models.Task Find(params object[] keyValues)
        {
            return this.SingleOrDefault(taskItem => taskItem.TaskId.Equals(keyValues.Single()));
        }
    }
}
