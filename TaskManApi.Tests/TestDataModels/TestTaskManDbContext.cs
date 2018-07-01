using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManApi.Data;
using TaskManApi.Models;

namespace TaskManApi.Tests.TestDataModels
{
    public class TestTaskManDbContext : ITaskManDb
    {
        public TestTaskManDbContext()
        {
            //overriding to Test DbSet we have created
            Tasks = new TestTaskManDbSet();
        }

        public IDbSet<Models.Task> Tasks { get; set; }


        public void Dispose()
        {
            //do nothing
        }

        public void SaveChanges()
        {
            //do nothing
        }
    }
}
