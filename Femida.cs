//Версия 0.1
//Название: Femida
//Описание: Femida - это античит для игры террария предназначенный для администраторов серверов.


// ███████╗███████╗███╗   ███╗██╗██████╗  █████╗ 
// ██╔════╝██╔════╝████╗ ████║██║██╔══██╗██╔══██╗
// █████╗  █████╗  ██╔████╔██║██║██║  ██║███████║   By WoodMan with <3
// ██╔══╝  ██╔══╝  ██║╚██╔╝██║██║██║  ██║██╔══██║
// ██║     ███████╗██║ ╚═╝ ██║██║██████╔╝██║  ██║
// ╚═╝     ╚══════╝╚═╝     ╚═╝╚═╝╚═════╝ ╚═╝  ╚═╝
                                              

// Модули 

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;


namespace Femida
{
    [ApiVersion(2, 1)] ////////////////////Версия API 2,1
    public class Femida : TerrariaPlugin
    {
        public Femida(Main game)
            : base(game)
        {
            Order = -4; //////////////////////////// тут -4
        }

        public override void Initialize()
        {
			Commands.ChatCommands.Add(new Command("tshock.admin.ban", OfflineBan, "fban")); /////////////////////команды без пробела
        }

   //     public override string Version
   //     {
			//get { return "1.0"; }           ////////////////////Такого Нет 
   //     }

        public override string Name
{
            get { return "Femida"; }
        }

        public override string Author
{
            get { return "woodman"; }
        }

        public override string Description
{
            get { return "Античит для админов серверов в Террарии."; }
        }

        private static void OfflineBan(CommandArgs args)
        {

            if (args.Parameters.Count < 1 || args.Parameters[0].ToLower() == "help")
			{
				args.Player.SendInfoMessage("Syntax: /fban  add \"имя\" [reason]");
				return;
			}

			if (args.Parameters[0].ToLower() != "add")
			{
				#region Команды по-умолчанию
				args.Player.SendInfoMessage("Используйте /ban.");
                return;
				#endregion Команды по-умолчанию
			}

			if (args.Parameters.Count >= 2)
			{
				#region Добавить бан
				string plStr = args.Parameters[1];
				var player = TShock.UserAccounts.GetUserAccountByName(plStr); 

				if (player == null)
				{
					args.Player.SendErrorMessage("Пользователь введен неверно!");
					return;
				}
				else
				{
					string reason = args.Parameters.Count > 2
									? String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2))
									: "no reason.";
					bool force = !args.Player.RealPlayer;
					string adminUserName = args.Player.Name;
					adminUserName = String.IsNullOrWhiteSpace(adminUserName) ? args.Player.Name : adminUserName;
					if (force || !TShock.Groups.GetGroupByName(player.Group).HasPermission(Permissions.immunetoban ))
					{
						List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(player.KnownIps);
						string ip = KnownIps[KnownIps.Count - 1];
						string uuid = player.UUID;
						string playerName = player.Name;
						string identf = $"{Identifier.Account}{playerName}";
						DateTime expiration = DateTime.MaxValue;
						AddBanResult addBanResult = TShock.Bans.InsertBan(identf, reason, playerName, DateTime.UtcNow, expiration);
						var players = TShockAPI.TSPlayer.FindByNameOrID(player.Name);
						
						if (players.Count == 1) players[0].Disconnect(string.Format("Забанен!!!: {0}", reason));
						Console.WriteLine(string.Format("{0} был наказан администратором {1} по причине: '{2}'", playerName, adminUserName, reason));
						string verb = force ? "force-" : "";
						if (String.IsNullOrWhiteSpace(adminUserName))
							TSPlayer.All.SendInfoMessage((string.Format("игрок {0} был наказан. Причина - '{1}'", playerName, reason.ToLower())));
						else
							TSPlayer.All.SendInfoMessage(string.Format("{0} был наказан администратором {1} по причине: '{2}'", playerName, adminUserName, reason));
					}
					else
					{
						args.Player.SendErrorMessage("Вы не можете забанить другого администратора!");
					}
				}
				return;
				#endregion Добавить бан!
				
            }
			args.Player.SendInfoMessage("Syntax: /fban  add \"name\" [reason]");
        }
    }
}
