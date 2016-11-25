﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tera.Game.Messages;

namespace Tera.Game
{
    // Creates a ParsedMessage from a Message
    // Contains a mapping from OpCodeNames to message types and knows how to instantiate those
    // Since it works with OpCodeNames not numeric OpCodes, it needs an OpCodeNamer
    public class MessageFactory
    {
        private static readonly Delegate UnknownMessageDelegate = Helpers.Contructor<Func<TeraMessageReader, UnknownMessage>>();
        private static readonly Dictionary<string, Delegate> OpcodeNameToType = new Dictionary<string, Delegate> {{"C_CHECK_VERSION", Helpers.Contructor<Func<TeraMessageReader, C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate> CoreServices = new Dictionary<string, Delegate>
        {
            {"C_CHECK_VERSION", Helpers.Contructor<Func<TeraMessageReader,C_CHECK_VERSION>>()},
            {"S_EACH_SKILL_RESULT", Helpers.Contructor<Func<TeraMessageReader,EachSkillResultServerMessage>>()},
            {"S_SPAWN_USER", Helpers.Contructor<Func<TeraMessageReader,SpawnUserServerMessage>>()},
            {"S_SPAWN_ME", Helpers.Contructor<Func<TeraMessageReader,SpawnMeServerMessage>>()},
            {"S_SPAWN_NPC", Helpers.Contructor<Func<TeraMessageReader,SpawnNpcServerMessage>>()},
            {"S_SPAWN_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,SpawnProjectileServerMessage>>()},
            {"S_LOGIN", Helpers.Contructor<Func<TeraMessageReader,LoginServerMessage>>()},
            {"S_TARGET_INFO", Helpers.Contructor<Func<TeraMessageReader,STargetInfo>>()},
            {"S_START_USER_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,StartUserProjectileServerMessage>>()},
            {"S_CREATURE_CHANGE_HP", Helpers.Contructor<Func<TeraMessageReader,SCreatureChangeHp>>()},
            {"S_BOSS_GAGE_INFO", Helpers.Contructor<Func<TeraMessageReader,S_BOSS_GAGE_INFO>>()},
            {"S_NPC_TARGET_USER", Helpers.Contructor<Func<TeraMessageReader,SNpcTargetUser>>()},
            {"S_NPC_OCCUPIER_INFO", Helpers.Contructor<Func<TeraMessageReader,SNpcOccupierInfo>>()},
            {"S_ABNORMALITY_BEGIN", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityBegin>>()},
            {"S_ABNORMALITY_END", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityEnd>>()},
            {"S_ABNORMALITY_REFRESH", Helpers.Contructor<Func<TeraMessageReader,SAbnormalityRefresh>>()},
            {"S_DESPAWN_NPC", Helpers.Contructor<Func<TeraMessageReader,SDespawnNpc>>()},
            {"S_PLAYER_CHANGE_MP", Helpers.Contructor<Func<TeraMessageReader,SPlayerChangeMp>>()},
            {"S_PARTY_MEMBER_ABNORMAL_ADD", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalAdd>>()},
            {"S_PARTY_MEMBER_CHANGE_MP", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberChangeMp>>()},
            {"S_PARTY_MEMBER_CHANGE_HP", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberChangeHp>>()},
            {"S_PARTY_MEMBER_ABNORMAL_CLEAR", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalClear>>()},
            {"S_PARTY_MEMBER_ABNORMAL_DEL", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalDel>>()},
            {"S_PARTY_MEMBER_ABNORMAL_REFRESH", Helpers.Contructor<Func<TeraMessageReader,SPartyMemberAbnormalRefresh>>()},
            {"S_DESPAWN_USER", Helpers.Contructor<Func<TeraMessageReader,SDespawnUser>>()},
            {"S_USER_STATUS", Helpers.Contructor<Func<TeraMessageReader,SUserStatus>>()},
            {"S_CREATURE_LIFE", Helpers.Contructor<Func<TeraMessageReader,SCreatureLife>>()},
            {"S_CREATURE_ROTATE", Helpers.Contructor<Func<TeraMessageReader,S_CREATURE_ROTATE>>()},
            {"S_NPC_STATUS", Helpers.Contructor<Func<TeraMessageReader,SNpcStatus>>()},
            {"S_NPC_LOCATION", Helpers.Contructor<Func<TeraMessageReader,SNpcLocation>>()},
            {"S_USER_LOCATION", Helpers.Contructor<Func<TeraMessageReader,S_USER_LOCATION>>()},
            {"C_PLAYER_LOCATION", Helpers.Contructor<Func<TeraMessageReader,C_PLAYER_LOCATION>>()},
            {"S_INSTANT_MOVE", Helpers.Contructor<Func<TeraMessageReader,S_INSTANT_MOVE>>()},
            {"S_ACTION_STAGE", Helpers.Contructor<Func<TeraMessageReader,S_ACTION_STAGE>>()},
            {"S_ACTION_END", Helpers.Contructor<Func<TeraMessageReader,S_ACTION_END>>()},
            {"S_CHANGE_DESTPOS_PROJECTILE", Helpers.Contructor<Func<TeraMessageReader,S_CHANGE_DESTPOS_PROJECTILE>>()},
            {"S_PARTY_MEMBER_STAT_UPDATE", Helpers.Contructor<Func<TeraMessageReader,S_PARTY_MEMBER_STAT_UPDATE>>()},
            {"S_PLAYER_STAT_UPDATE", Helpers.Contructor<Func<TeraMessageReader,S_PLAYER_STAT_UPDATE>>()},
            {"S_PARTY_MEMBER_LIST", Helpers.Contructor<Func<TeraMessageReader,S_PARTY_MEMBER_LIST>>()},
            {"S_LEAVE_PARTY_MEMBER", Helpers.Contructor<Func<TeraMessageReader,S_LEAVE_PARTY_MEMBER>>()},
            {"S_BAN_PARTY_MEMBER", Helpers.Contructor<Func<TeraMessageReader,S_BAN_PARTY_MEMBER>>()},
            {"S_LEAVE_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_LEAVE_PARTY>>()},
            {"S_BAN_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_BAN_PARTY>>()},
            {"S_GET_USER_LIST", Helpers.Contructor<Func<TeraMessageReader,S_GET_USER_LIST>>()},
            {"S_GET_USER_GUILD_LOGO", Helpers.Contructor<Func<TeraMessageReader,S_GET_USER_GUILD_LOGO>>()},
            {"S_MOUNT_VEHICLE_EX", Helpers.Contructor<Func<TeraMessageReader,S_MOUNT_VEHICLE_EX>>() },
            {"S_CREST_INFO", Helpers.Contructor<Func<TeraMessageReader,S_CREST_INFO>>() },
        };

        private static readonly Dictionary<string, Delegate> ChatServices = new Dictionary<string, Delegate>
        {
            {"S_START_COOLTIME_SKILL", Helpers.Contructor<Func<TeraMessageReader,S_START_COOLTIME_SKILL>>()},
            {"S_CHAT", Helpers.Contructor<Func<TeraMessageReader,S_CHAT>>()},
            {"S_WHISPER", Helpers.Contructor<Func<TeraMessageReader,S_WHISPER>>()},
            {"S_TRADE_BROKER_DEAL_SUGGESTED", Helpers.Contructor<Func<TeraMessageReader,S_TRADE_BROKER_DEAL_SUGGESTED>>()},
            {"S_OTHER_USER_APPLY_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_OTHER_USER_APPLY_PARTY>>() },
            {"S_PRIVATE_CHAT", Helpers.Contructor<Func<TeraMessageReader,S_PRIVATE_CHAT>>() },
            {"S_FIN_INTER_PARTY_MATCH", Helpers.Contructor<Func<TeraMessageReader,S_FIN_INTER_PARTY_MATCH>>() },
            {"S_BATTLE_FIELD_ENTRANCE_INFO", Helpers.Contructor<Func<TeraMessageReader,S_BATTLE_FIELD_ENTRANCE_INFO>>() },
            {"S_REQUEST_CONTRACT", Helpers.Contructor<Func<TeraMessageReader,S_REQUEST_CONTRACT>>() },
            {"S_BEGIN_THROUGH_ARBITER_CONTRACT", Helpers.Contructor<Func<TeraMessageReader,S_BEGIN_THROUGH_ARBITER_CONTRACT>>() },
            {"S_CHECK_TO_READY_PARTY", Helpers.Contructor<Func<TeraMessageReader,S_CHECK_TO_READY_PARTY>>() },
            {"S_GUILD_QUEST_LIST", Helpers.Contructor<Func<TeraMessageReader,S_GUILD_QUEST_LIST>>() },
            {"S_START_GUILD_QUEST", Helpers.Contructor<Func<TeraMessageReader, S_START_GUILD_QUEST>>() }
        };


        private readonly OpCodeNamer _opCodeNamer;
        public string Version;
        public bool ChatEnabled {
            get { return _chatEnabled; }
            set
            {
                _chatEnabled = value;
                OpcodeNameToType.Clear();
                CoreServices.ToList().ForEach(x => OpcodeNameToType[x.Key] = x.Value);
                if (_chatEnabled) ChatServices.ToList().ForEach(x => OpcodeNameToType[x.Key] = x.Value);
            }
        }

        private bool _chatEnabled;

        public MessageFactory(OpCodeNamer opCodeNamer, string version, bool chatEnabled=false)
        {
            OpcodeNameToType.Clear();
            CoreServices.ToList().ForEach(x=>OpcodeNameToType[x.Key]=x.Value);
            if (chatEnabled) ChatServices.ToList().ForEach(x => OpcodeNameToType[x.Key] = x.Value);
            _opCodeNamer = opCodeNamer;
            Version = version;
            _chatEnabled = chatEnabled;
            foreach (var name in OpcodeNameToType.Keys)
            {
                opCodeNamer.GetCode(name);
            }
        }

        public MessageFactory()
        {
            _opCodeNamer = new OpCodeNamer(new Dictionary<ushort,string>{{19900 , "C_CHECK_VERSION" }} );
            Version = "Unknown";
        }

        private ParsedMessage Instantiate(string opCodeName, TeraMessageReader reader)
        {
            Delegate type;
            if (!OpcodeNameToType.TryGetValue(opCodeName, out type))
                type = UnknownMessageDelegate;
            return (ParsedMessage) type.DynamicInvoke(reader);
        }

        public ParsedMessage Create(Message message)
        {
            var reader = new TeraMessageReader(message, _opCodeNamer, Version);
            var opCodeName = _opCodeNamer.GetName(message.OpCode);
            return Instantiate(opCodeName, reader);
        }
    }
}