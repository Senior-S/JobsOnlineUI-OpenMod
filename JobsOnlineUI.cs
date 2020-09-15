using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Plugins;
using OpenMod.API.Plugins;
using SDG.Unturned;
using OpenMod.API.Permissions;

[assembly: PluginMetadata("SS.JobsOnlineUI", DisplayName = "JobsOnlineUI")]
namespace JobsOnlineUI
{
    public class JobsOnlineUI : OpenModUnturnedPlugin
    {
        private readonly ILogger<JobsOnlineUI> ro_Logger;
        private readonly IConfiguration ro_configuration;
        private readonly IPermissionRegistry ro_permissionRegistry;

        public JobsOnlineUI(
            ILogger<JobsOnlineUI> logger,
            IConfiguration configuration,
            IPermissionRegistry permissionRegistry,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ro_Logger = logger;
            ro_configuration = configuration;
            ro_permissionRegistry = permissionRegistry;
        }

        protected override async UniTask OnLoadAsync()
        {
			await UniTask.SwitchToMainThread();
            PlayerInput.onPluginKeyTick += OnKeyPressed;
            ro_permissionRegistry.RegisterPermission(this, ro_configuration.GetSection("plugin_configuration:medic_permission").Get<string>());
            ro_permissionRegistry.RegisterPermission(this, ro_configuration.GetSection("plugin_configuration:police_permission").Get<string>());
            ro_Logger.LogInformation("Plugin loaded correctly!");
            ro_Logger.LogInformation("If you have any error you can contact the owner in discord: Senior S#9583");
        }

        private void OnKeyPressed(Player player, uint simulation, byte key, bool state)
        {
            if (key == 4 && state)
            {
                EffectManager.sendUIEffect(28000, 280, player.channel.owner.playerID.steamID, true, Jobs.Civil + "", Jobs.Police + "", Jobs.Medic + "");
            }
            else if (key == 4 && !state)
            {
                EffectManager.askEffectClearByID(28000, player.channel.owner.playerID.steamID);
            }
        }

        protected override async UniTask OnUnloadAsync()
        {
            await UniTask.SwitchToMainThread();
            PlayerInput.onPluginKeyTick -= OnKeyPressed;
            ro_Logger.LogInformation("Plugin unloaded correctly!");
        }
    }
}
