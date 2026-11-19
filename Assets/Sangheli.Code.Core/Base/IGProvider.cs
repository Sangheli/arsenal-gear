using System.Collections.Generic;

public interface IGProvider
{
    List<IBaseInit> GetInitableList();
}
