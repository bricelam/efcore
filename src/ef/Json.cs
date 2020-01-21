// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.DotNet.Cli.CommandLine;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools
{
    internal static class Json
    {
        public static CommandOption ConfigureOption(CommandLineApplication command)
            => command.Option("--json", Resources.JsonDescription);

        public static string Literal(object value)
            => value switch
            {
                null => "null",
                string text => "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"",
                bool flag => flag ? "true" : "false",
                _ => throw new Exception("Unexpected type: " + value.GetType())
            };
    }
}
