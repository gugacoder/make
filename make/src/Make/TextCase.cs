using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Make
{
  [Flags]
  public enum TextCase
  {
    Default = 0x000,
    KeepOriginal = 0x001,

    // capitalização
    UpperCase = 0x002,
    LowerCase = 0x004,
    ProperCase = 0x008,

    // separação
    Hyphenated = 0x010,
    Underscore = 0x020,
    Dotted = 0x040,
    Spaced = 0x080,
    Joined = 0x100,

    // casos especiais
    CamelCase = 0x200 | ProperCase | Joined,

    // desativa a preservação de delimitadores antes e depois.
    // por exemplo: "__id" se torna "id".
    NoPrefix = 0x1000,

    // composições
    AllCaps = UpperCase | Underscore,
    PascalCase = ProperCase | Joined,
  }
}
