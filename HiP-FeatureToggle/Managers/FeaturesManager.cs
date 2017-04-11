using PaderbornUniversity.SILab.Hip.FeatureToggle.Data;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Managers
{
    public class FeaturesManager
    {
        private readonly ToggleDbContext _db;

        public FeaturesManager(ToggleDbContext db)
        {
            _db = db;
        }

        public IQueryable<Feature> GetFeatures(bool loadParent = false, bool loadChildren = false, bool loadGroups = false)
        {
            return _db.Features
                .IncludeIf(loadParent, nameof(Feature.Parent))
                .IncludeIf(loadChildren, nameof(Feature.Children))
                .IncludeIf(loadGroups, nameof(Feature.GroupsWhereEnabled));
        }

        public Feature GetFeature(int featureId, bool loadParent = false, bool loadChildren = false, bool loadGroups = false)
        {
            return GetFeatures(loadParent, loadChildren, loadGroups)
                .FirstOrDefault(f => f.Id == featureId);
        }

        /// <exception cref="ArgumentNullException">The specified arguments are null</exception>
        /// <exception cref="ArgumentException">A feature with the specified name already exists</exception>
        /// <exception cref="ResourceNotFoundException{Feature}">The referenced parent feature does not exist</exception>
        public Feature CreateFeature(FeatureArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (_db.Features.Any(f => f.Name == args.Name))
                throw new ArgumentException($"A feature with name '{args.Name}' already exists");

            var feature = new Feature { Name = args.Name };

            if (args.Parent != null)
            {
                feature.Parent = GetFeature(args.Parent.Value);

                if (feature.Parent == null)
                    throw new ResourceNotFoundException<Feature>(args.Parent);
            }

            _db.Features.Add(feature);
            _db.SaveChanges();
            return feature;
        }

        /// <exception cref="ResourceNotFoundException{Feature}">The feature with specified ID does not exist</exception>
        public void DeleteFeature(int featureId)
        {
            var feature = GetFeature(featureId, loadParent: true, loadChildren: true, loadGroups: true);

            if (feature == null)
                throw new ResourceNotFoundException<Feature>(featureId);

            var mappings = feature.GroupsWhereEnabled.ToList();
            var groups = feature.GroupsWhereEnabled.Select(m => m.Group).ToList();

            // 1) remove feature from groups where it is enabled
            //foreach (var group in feature.GroupsWhereEnabled.ToList())
                // TODO: Do we have to explicitly delete mapping? (_db.MyMappings.Remove(...))

            _db.SaveChanges();
        }

        /// <exception cref="ArgumentNullException">The specified arguments are null</exception>
        /// <exception cref="ArgumentException">The new feature name is already in use</exception>
        /// <exception cref="ResourceNotFoundException{Feature}">There is no feature with the specified ID</exception>
        public void UpdateFeature(int featureId, FeatureArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (_db.Features.Any(f => f.Name == args.Name && f.Id != featureId))
                throw new ArgumentException($"A feature with name '{args.Name}' already exists");

            var feature = GetFeature(featureId, loadParent: true, loadChildren: true, loadGroups: true);

            if (feature == null)
                throw new ResourceNotFoundException<Feature>(featureId);

            feature.Name = args.Name;

            // TODO

            _db.SaveChanges();
        }
    }
}
