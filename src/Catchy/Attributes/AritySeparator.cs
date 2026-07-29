using System.ComponentModel;

namespace __
{
    // Arity separator [__._] Needed to separate value string arguments from expression string arguments
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct _ { private _(int _) { } }

}
