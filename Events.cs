using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Users.Events;
using OpenMod.Unturned.Users;
using OpenMod.API.Permissions;
using Microsoft.Extensions.Configuration;

namespace JobsOnlineUI
{
    public static class Jobs
    {
        public static int Civil;
        public static int Medic;
        public static int Police;
    }

    public class PlayerJoined: IEventListener<IUserConnectedEvent>
    {
        private readonly IPermissionChecker ro_PermissionChecker;
        private readonly IConfiguration ro_Configuration;

        public PlayerJoined(IPermissionChecker permissionChecker, IConfiguration configuration)
        {
            ro_PermissionChecker = permissionChecker;
            ro_Configuration = configuration;
        }

        public async Task HandleEventAsync(object sender, IUserConnectedEvent @event)
        {
            UnturnedUser user = (UnturnedUser)@event.User;
            Jobs.Civil++;
            PermissionGrantResult medic = await ro_PermissionChecker.CheckPermissionAsync(user, ro_Configuration.GetSection("plugin_configuration:medic_permission").Get<string>());
            PermissionGrantResult police = await ro_PermissionChecker.CheckPermissionAsync(user, ro_Configuration.GetSection("plugin_configuration:police_permission").Get<string>());

            if (medic == PermissionGrantResult.Grant && !user.Player.Player.channel.owner.isAdmin)
            {
                Jobs.Medic++;
            }
            if (police == PermissionGrantResult.Grant && !user.Player.Player.channel.owner.isAdmin)
            {
                Jobs.Police++;
            }
        }

        public class PlayerQuit : IEventListener<IUserDisconnectedEvent>
        {
            private readonly IPermissionChecker ro_PermissionChecker;
            private readonly IConfiguration ro_Configuration;

            public PlayerQuit(IPermissionChecker permissionChecker, IConfiguration configuration)
            {
                ro_PermissionChecker = permissionChecker;
                ro_Configuration = configuration;
            }

            public async Task HandleEventAsync(object sender, IUserDisconnectedEvent @event)
            {
                UnturnedUser user = (UnturnedUser)@event.User;
                Jobs.Civil--;
                var medic = await ro_PermissionChecker.CheckPermissionAsync(user, ro_Configuration.GetSection("plugin_configuration:medic_permission").Get<string>());
                var police = await ro_PermissionChecker.CheckPermissionAsync(user, ro_Configuration.GetSection("plugin_configuration:police_permission").Get<string>());

                if (medic == PermissionGrantResult.Grant && !user.Player.Player.channel.owner.isAdmin && Jobs.Medic > 0)
                {
                    Jobs.Medic--;
                }
                if (police == PermissionGrantResult.Grant && !user.Player.Player.channel.owner.isAdmin && Jobs.Police > 0)
                {
                    Jobs.Police--;
                }
            }
        }
    }
}
