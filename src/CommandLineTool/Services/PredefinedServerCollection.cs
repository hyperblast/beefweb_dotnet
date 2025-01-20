using System;
using System.Collections.Generic;

namespace Beefweb.CommandLineTool.Services;

public class PredefinedServerCollection() : Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);
