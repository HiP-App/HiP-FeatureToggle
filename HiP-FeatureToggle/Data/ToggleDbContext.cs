using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Data
{
    public class ToggleDbContext: DbContext
    {
        /*
         * TODO: add DbSets
         * Example:
         * public DbSet<AssociatedTopic> AssociatedTopics { get; set; }
         */

		public DbSet<Values> Values { get; set; }

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
