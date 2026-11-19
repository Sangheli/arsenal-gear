using System.Collections.Generic;

public interface IBfsTile
{
    List<IBfsTile> ConnectedTileList { get; }
}
