using System.Data.Entity;
using TaskManApi.Models;

namespace TaskManApi.Data
{
    public interface ITaskManDb
    {
        IDbSet<Task> Tasks { get; set; }
        void SaveChanges();
        void Dispose();
    }
}