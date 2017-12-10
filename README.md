# HiP-FeatureToggle

Microservice that implements feature toggles as described in our QA workflow.

See [the graphs page](https://github.com/HiP-App/HiP-CmsWebApi/graphs/contributors) 
for a list of code contributions.

# Getting started

See [Getting Started](https://github.com/HiP-App/HiP-CmsWebApi#getting-started) on the CmsWebAPI project.

# Core Concepts

The service manages a hierarchy (tree) of *features* and a list of *feature groups*. A feature consists of an ID and a descriptive name (e.g. "Can See Seminar Content"). A feature group (better thought of as a "user group") consists of an ID, a name (e.g. "Beta testers"), a list of users ("members") belonging to that group, and a list of features that are enabled for the members of the group. Each user belongs to exactly one group. There are two special, pre-defined groups:

* 'Default': If a user has not (yet) been assigned to a group, he/she is in the default group. Furthermore, deleting a group moves all its members to the 'Default'-group again.
* 'Public': This group only applies to unauthenticated, anonymous users. Users cannot be explicitly assigned to this group.

'Default' and 'Public' are *protected* groups which means that they cannot be renamed or deleted.

Features can be enabled and disabled for individual groups (but not for individual users). A feature is considered *effectively enabled* for a particular user if it is effectively enabled for his/her group. A feature is considered *effectively enabled* for a group if the feature itself as well as all ancestor features in the hierarchy (parent, parent's parent, ...) are enabled for that group.

# How to submit Defects and Feature Proposals

Please write an email to [hip-app@campus.upb.de](mailto:hip-app@campus.upb.de).

## Contact

> HiP (History in Paderborn) ist eine Plattform der:
> Universität Paderborn
> Warburger Str. 100
> 33098 Paderborn
> http://www.uni-paderborn.de
> Tel.: +49 5251 60-0

You can also [write an email](mailto:hip-app@campus.upb.de).
