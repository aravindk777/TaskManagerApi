namespace TaskMan.Data.Entities
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Data.Entity.Migrations;
    using System.Configuration;

    public partial class TaskManagerContext : DbContext
    {
        public TaskManagerContext()
            : base(ConfigurationManager.ConnectionStrings["TaskManagerDb"].Name)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TaskManagerContext>());
        }

        public virtual DbSet<MyTask> Tasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /* Have to admit: after lots of research to ensure single table only and 
             * not two or multiple just for this, i was stalling the logic for a while and finally found the below one:
             * To even emphasize: **LEARNED AND** referenced from:
             * { 
             *      www.dzone.com/articles/using-self-referencing-tables & 
             *      blogs.ms.co.il/gilf/2011/06/03/how-to-configure-a-self-referencing-entity-in-code-first/ 
             * }
             */
            modelBuilder
                .Entity<MyTask>()
                    .HasOptional(parent => parent.ParentTask)
                    .WithMany()
                    .HasForeignKey(fk => fk.ParentTaskId)
                    .WillCascadeOnDelete(false); // <-- this is VERY VERY Important for me to set this as I dont want to delete my child tasks just because the parent is deleted

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
