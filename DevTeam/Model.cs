using System;
using System.Collections.Generic;

public abstract class BaseEntity {
    protected BaseEntity() {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
}
public enum JobTitle {
    Developer,
    ScrumMaster,
    ProjectOwner
}

public class Team : BaseEntity {
    public string Name { get; set; }
    public virtual ICollection<TeamMember> TeamMembers { get; set; }
    public override string ToString()
        => String.Format("Team \"{0}\", # of members: {1}", Name, TeamMembers.Count);
}

public class TeamMember : BaseEntity {
    public Guid? TeamId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public JobTitle Title { get; set; }
    public virtual Team Team { get; set; }

    public override string ToString()
        => String.Format("\tMember \"{0} {1}\": {2}", FirstName, LastName, Title.ToString());
}
