using System;

namespace Beefweb.CommandLineTool.Services;

public class InvalidRequestException(string message) : Exception(message);
