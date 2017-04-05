using Microsoft.EntityFrameworkCore;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Data;
using PaderbornUniversity.SILab.Hip.FeatureToggle.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.FeatureToggle.Managers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// There exists a default feature group new users are assigned to.
    /// </remarks>
    public class FeatureGroupsManager
    {
        private static readonly User[] _noUsers = new User[0];
        private static readonly Feature[] _noFeatures = new Feature[0];

        private readonly ToggleDbContext _db;

        public FeatureGroup DefaultGroup { get; }

        public FeatureGroupsManager(ToggleDbContext db)
        {
            _db = db;
            DefaultGroup = db.FeatureGroups.Single(g => g.Name == "Default");
        }

        public User GetOrCreateUser(string userId)
        {
            var user = _db.Users
                .Include(nameof(User.FeatureGroup))
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
                return user;

            // create new user
            var newUser = CreateUser(userId);
            _db.SaveChanges();
            return newUser;
        }

        /// <exception cref="ArgumentException">No features exist for one or multiple of the specified IDs</exception>
        public IReadOnlyCollection<Feature> GetFeatures(IEnumerable<int> featureIds)
        {
            if (featureIds == null)
                return _noFeatures;

            var featureIdsSet = featureIds.ToSet();
            var storedFeatures = _db.Features.Where(f => featureIdsSet.Contains(f.Id)).ToList();
            var missingFeatureIds = featureIdsSet.Except(storedFeatures.Select(f => f.Id));

            if (missingFeatureIds.Any())
                throw new ArgumentException("The following features do not exist: " + string.Join(", ", missingFeatureIds));

            return storedFeatures;
        }

        public IEnumerable<FeatureGroup> GetGroups(bool loadMembers = false, bool loadFeatures = false)
        {
            return _db.FeatureGroups
                .IncludeIf(loadMembers, nameof(FeatureGroup.Members))
                .IncludeIf(loadFeatures, nameof(FeatureGroup.EnabledFeatures));
        }

        public FeatureGroup GetGroup(int groupId, bool loadMembers = false, bool loadFeatures = false)
        {
            return GetGroups(loadMembers, loadFeatures)
                .FirstOrDefault(g => g.Id == groupId);
        }

        /// <exception cref="ArgumentNullException">The specified group is null</exception>
        /// <exception cref="ArgumentException">A feature group with the specified name already exists</exception>
        public void AddGroup(FeatureGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (_db.FeatureGroups.Any(g => g.Name == group.Name))
                throw new ArgumentException($"A feature group with name '{group.Name}' already exists");

            _db.FeatureGroups.Add(group);
            _db.SaveChanges();
        }

        public bool RemoveGroup(int groupId)
        {
            // the default group itself can't be removed
            if (groupId == DefaultGroup.Id)
                return false;

            var group = GetGroup(groupId, loadMembers: true);

            if (group == null)
                return false;

            // before removing, move all group members to the default group
            foreach (var member in group.Members.ToList())
                MoveUserToGroup(member, DefaultGroup);

            _db.FeatureGroups.Remove(group);
            _db.SaveChanges();
            return true;
        }

        /// <exception cref="ArgumentNullException">Any argument is null</exception>
        public void MoveUserToGroup(User user, FeatureGroup group)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (group == null)
                throw new ArgumentNullException(nameof(group));

            MoveUserToGroupCore(user, group);
            _db.SaveChanges();
        }

        private void MoveUserToGroupCore(User user, FeatureGroup group)
        {
            // remove user from current group, then add to new group
            user.FeatureGroup.Members.Remove(user);
            group.Members.Add(user);
            user.FeatureGroup = group;
        }

        private User CreateUser(string id)
        {
            var user = new User
            {
                Id = id,
                FeatureGroup = DefaultGroup
            };

            _db.Users.Add(user);
            return user;
        }
    }

    internal static class LinqExtensions
    {
        public static ISet<T> ToSet<T>(this IEnumerable<T> collection) => new HashSet<T>(collection);

        public static IQueryable<T> IncludeIf<T>(this IQueryable<T> query, bool include, string navigationPropertyPath)
            where T : class
        {
            return include ? query.Include(navigationPropertyPath) : query;
        }
    }
}
