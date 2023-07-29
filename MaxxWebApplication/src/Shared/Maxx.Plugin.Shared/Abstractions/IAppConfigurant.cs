using Microsoft.Extensions.Hosting;

namespace Maxx.Plugin.Shared.Abstractions;

public interface IAppConfigurant
{
    void Configure(IHost host);
}
