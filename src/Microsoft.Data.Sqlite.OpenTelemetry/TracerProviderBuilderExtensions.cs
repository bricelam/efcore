// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.Data.Sqlite.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTelemetry.Trace;

/// <summary>
/// OpenTelemetry extensions for Microsoft.Data.Sqlite.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    /// <summary>
    /// Enables Microsoft.Data.Sqlite instrumentation.
    /// </summary>
    /// <param name="builder">The <see cref="TracerProviderBuilder"/> being configured.</param>
    /// <param name="name">The name to use when retrieving options.</param>
    /// <param name="configureOptions">An action to configure the options.</param>
    /// <returns>The same <see cref="TracerProviderBuilder"/> instance to chain additional calls.</returns>
    public static TracerProviderBuilder AddSqliteInstrumentation(
        this TracerProviderBuilder builder,
        string? name = null,
        Action<SqliteInstrumentationOptions>? configureOptions = null)
    {
        //name ??= Options.DefaultName;

        builder.ConfigureServices(
            services =>
            {
                if (configureOptions is not null)
                {
                    services.Configure(name, configureOptions);
                }

                //services.AddSingleton<IOptionsFactory<SqliteInstrumentationOptions>>(
                //    sp => new DelegatingOptionsFactory<T>(
                //            (c, n) => optionsFactoryFunc!(sp, c, n),
                //            sp.GetRequiredService<IConfiguration>(),
                //            sp.GetServices<IConfigureOptions<T>>(),
                //            sp.GetServices<IPostConfigureOptions<T>>(),
                //            sp.GetServices<IValidateOptions<T>>()));
            });

        builder.AddSource(SqliteActivitySourceHelper.ActivitySourceName);
        builder.AddProcessor<SqliteActivityProcessor>();

        return builder;
    }

    private class SqliteActivityProcessor : BaseProcessor<Activity>
    {
        readonly SqliteInstrumentationOptions _options;

        public SqliteActivityProcessor(SqliteInstrumentationOptions options)
            => _options = options;

        public override void OnEnd(Activity data)
        {
            if (!_options.SetDbStatement)
            {
                data.SetTag("db.statement", null);
            }
            
            //if (!_options.RecordException)
            //{
            //    // TODO: No way to remove events. Is there a way to flow all the data into here and only add the event when the optoin is set?    
            //}
        }
    }
}
