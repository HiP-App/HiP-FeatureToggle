using Microsoft.EntityFrameworkCore;

namespace de.uni_paderborn.si_lab.hip.featuretoggle.data
{
    public class ToggleDbContext: DbContext
    {
        /*
         * TODO: add DbSets
         * Example:
         * public DbSet<AssociatedTopic> AssociatedTopics { get; set; }
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
             * TODO: create any mappings that you need
             * Example:
             * modelBuilder.Entity<User>().HasIndex(b => b.Email).IsUnique();
             * new AssociatedTopicMap(modelBuilder.Entity<AssociatedTopic>());
             */
        }
    }
}
