namespace TaskManApi.Data
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using TaskManApi.Models;

    public class TaskManDb : DbContext
    {
        // Your context has been configured to use a 'TaskManDb' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'TaskManApi.Data.TaskManDb' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'TaskManDb' 
        // connection string in the application configuration file.
        public TaskManDb()
            : base("name=TaskManDb")
        {
            Database.SetInitializer<TaskManDb>(new DropCreateDatabaseIfModelChanges<TaskManDb>());
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}